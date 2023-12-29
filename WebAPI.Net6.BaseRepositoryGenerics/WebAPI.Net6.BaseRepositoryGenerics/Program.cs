using Autofac.Extensions.DependencyInjection;
using Elasticsearch.Net;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;
using System.Configuration;
using System.IO.Compression;
using System.Reflection;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Cache;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Secure;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure;
using WebAPI.Net6.BaseRepositoryGenerics.Repositories;
using WebAPI.Net6.BaseRepositoryGenerics.Repositories.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Services;
using WebAPI.Net6.BaseRepositoryGenerics.Services.Cache;
using WebAPI.Net6.BaseRepositoryGenerics.Services.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Services.Secure;
using Winton.Extensions.Configuration.Consul;

namespace WebAPI.Net6.BaseRepositoryGenerics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            // Add services to the container.

            builder.Services.AddControllers()
                .AddNewtonsoftJson(op => op.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Adds a custom error response factory when ModelState is invalid
                    options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.ProduceErrorResponse;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.ClearProviders();
            builder.Logging.AddLog4Net();

            // 日志记录
            builder.Host.UseSerilog();
            var localconfig = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile(builder.Environment.IsProduction() ? "appsettings.json" : $"appsettings.{builder.Environment.EnvironmentName}.json")
                                  .AddEnvironmentVariables()
                                  .Build();
            var consulAddress = localconfig["Consul:ConsulAddress"];
            var consulBaseKey = localconfig["Consul:BaseKey"];
            var consulToken = localconfig["Consul:Token"];

            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());

            builder.Configuration.AddConsul(consulBaseKey, op =>
            {
                op.ConsulConfigurationOptions = cco =>
                {
                    cco.Address = new Uri(consulAddress);
                    cco.Token = consulToken;
                };
                op.ReloadOnChange = true;
            });

            configuration.AddConsul(consulBaseKey, op =>
            {
                op.ConsulConfigurationOptions = cco =>
                {
                    cco.Address = new Uri(consulAddress);
                    cco.Token = consulToken;
                };
                op.ReloadOnChange = true;
            });
            ConfigHelper.configuration = configuration.Build();

            var redisConfig = $"{builder.Configuration.GetValue<string>("Redis:Server")}:{builder.Configuration.GetValue<int>("Redis:Port")}," +
                            $"password={builder.Configuration.GetValue<string>("Redis:Password")}";
            var redis = ConnectionMultiplexer.Connect(redisConfig);
            builder.Services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "Service.ProtectKey");

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
            });

            string esUrl = ConfigHelper.GetSectionValue("ElasticConfig:Service");
            string esUserName = ConfigHelper.GetSectionValue("ElasticConfig:UserName");
            string esPassword = ConfigHelper.GetSectionValue("ElasticConfig:Password");
            string EnvironmentName = ConfigHelper.GetSectionValue("ElasticConfig:EnvironmentName");
            var uri = new Uri(esUrl);
            var credentials = new BasicAuthenticationCredentials(esUserName, esPassword);

            var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Filter.ByIncludingOnly(evt => evt.MessageTemplate.Text == "{@log}")
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(uri)
                {
                    ModifyConnectionSettings = connection =>
                    {
                        if (!string.IsNullOrEmpty(esUserName) && !string.IsNullOrEmpty(esPassword))
                            connection.BasicAuthentication(esUserName, esPassword);
                        return connection;
                    },
                    AutoRegisterTemplate = true, // 如果需要自动注册模板
                    IndexFormat = $"webapi-serilog-{{0:yyyy.MM.dd}}"
                })
                .CreateLogger();


            Log.Logger = log;
            //Log.Information("这是一条测试日志");
            LoggerHelper._logger = log;

            var cipherService = ActivatorUtilities.CreateInstance<CipherService>(builder.Services.BuildServiceProvider());
            var dbConnString = builder.Configuration.GetSection("ConnectionStrings:ConnString").Value;
            if (dbConnString!.Contains("database=")) //连接串还未加密
                cipherService.Encrypt(dbConnString); //用于记录下来加密后的结果，用来修改配置文件中的数据库连接串
            else
                dbConnString = cipherService.Decrypt(dbConnString);

            builder.Services.AddDbContext<MyContext>(o => o.UseSqlServer(dbConnString), ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //builder.Services.AddSingleton<ICipherService, CipherService>();
            //builder.Services.AddSingleton<ICacheService, CacheService>();

            //builder.Services.AddSingleton<IErrorMessageRepository, ErrorMessageRepository>();
            //builder.Services.AddSingleton<IErrorMessageService, ErrorMessageService>();

            builder.Services.AddHttpClient();

            #region EnableCors

            // 配置跨域处理
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    // 允许任何来源的主机访问
                    builder.AllowAnyOrigin()

                    // 允许任何Method
                    .AllowAnyMethod()

                    // 允许任何Header
                    .AllowAnyHeader();

                    // .AllowCredentials();//指定处理cookie
                });
            });

            #endregion EnableCors

            #region MiniProfiler

            if (!builder.Environment.IsProduction())
            {
                // 首先添加一个配置选项，用于访问分析结果： https://localhost:7236/profiler/results
                builder.Services.AddMiniProfiler(options =>
                {
                    // 设定弹出窗口的位置是左下角
                    options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;

                    // 设定在弹出的明细窗口里会显式Time With Children这列
                    options.PopupShowTimeWithChildren = true;

                    // 设定访问分析结果URL的路由基地址
                    options.RouteBasePath = "/profiler";
                })

                // 然后在之前的配置后边加上AddEntityFramework()：
                .AddEntityFramework();
            }

            #endregion MiniProfiler

            #region NewtonsoftJson

            builder.Services.AddMvc().AddNewtonsoftJson(options =>
            {
                // 忽略空值
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                // 忽略默认值 options.SerializerSettings.DefaultValueHandling =
                // Newtonsoft.Json.DefaultValueHandling.Ignore; 忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // 设置为驼峰命名
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                // 自定义日期时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            #endregion NewtonsoftJson

            #region MediatR

            // Register MediatR services
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            #endregion MediatR

            #region FluentValidation

            // version 11.1 and newer
            builder.Services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = false;
            });

            #endregion FluentValidation

            #region Cors
            // 配置跨域处理
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    // 允许任何来源的主机访问
                    builder.AllowAnyOrigin()

                    // 允许任何Method
                    .AllowAnyMethod()

                    // 允许任何Header
                    .AllowAnyHeader();
                });
            });
            #endregion

            #region Compression
            // 压缩
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                // 可以添加多种压缩类型，程序会根据级别自动获取最优方式
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                // 针对指定的MimeType来使用压缩策略
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "application/json" });
            });
            // 针对不同的压缩类型，设置对应的压缩级别
            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                // 使用最快的方式进行压缩，不一定是压缩效果最好的方式
                options.Level = CompressionLevel.Fastest;

                // 不进行压缩操作
                // options.Level = CompressionLevel.NoCompression;

                // 即使需要耗费很长的时间，也要使用压缩效果最好的方式
                // options.Level = CompressionLevel.Optimal;
            });
            #endregion
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                // UseMiniProfiler.
                app.UseMiniProfiler();
            }

            // 启用跨域策略 最好在这个位置，app执行有先后顺序
            app.UseCors("any");// 最重要的一点是就是配置中间件在管道中的位置，一定要把它放在UseMvc()方法之前
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}