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

            // ��־��¼
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
                    AutoRegisterTemplate = true, // �����Ҫ�Զ�ע��ģ��
                    IndexFormat = $"webapi-serilog-{{0:yyyy.MM.dd}}"
                })
                .CreateLogger();


            Log.Logger = log;
            //Log.Information("����һ��������־");
            LoggerHelper._logger = log;

            var cipherService = ActivatorUtilities.CreateInstance<CipherService>(builder.Services.BuildServiceProvider());
            var dbConnString = builder.Configuration.GetSection("ConnectionStrings:ConnString").Value;
            if (dbConnString!.Contains("database=")) //���Ӵ���δ����
                cipherService.Encrypt(dbConnString); //���ڼ�¼�������ܺ�Ľ���������޸������ļ��е����ݿ����Ӵ�
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

            // ���ÿ�����
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    // �����κ���Դ����������
                    builder.AllowAnyOrigin()

                    // �����κ�Method
                    .AllowAnyMethod()

                    // �����κ�Header
                    .AllowAnyHeader();

                    // .AllowCredentials();//ָ������cookie
                });
            });

            #endregion EnableCors

            #region MiniProfiler

            if (!builder.Environment.IsProduction())
            {
                // �������һ������ѡ����ڷ��ʷ�������� https://localhost:7236/profiler/results
                builder.Services.AddMiniProfiler(options =>
                {
                    // �趨�������ڵ�λ�������½�
                    options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;

                    // �趨�ڵ�������ϸ���������ʽTime With Children����
                    options.PopupShowTimeWithChildren = true;

                    // �趨���ʷ������URL��·�ɻ���ַ
                    options.RouteBasePath = "/profiler";
                })

                // Ȼ����֮ǰ�����ú�߼���AddEntityFramework()��
                .AddEntityFramework();
            }

            #endregion MiniProfiler

            #region NewtonsoftJson

            builder.Services.AddMvc().AddNewtonsoftJson(options =>
            {
                // ���Կ�ֵ
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                // ����Ĭ��ֵ options.SerializerSettings.DefaultValueHandling =
                // Newtonsoft.Json.DefaultValueHandling.Ignore; ����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // ����Ϊ�շ�����
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                // �Զ�������ʱ���ʽ
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
            // ���ÿ�����
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    // �����κ���Դ����������
                    builder.AllowAnyOrigin()

                    // �����κ�Method
                    .AllowAnyMethod()

                    // �����κ�Header
                    .AllowAnyHeader();
                });
            });
            #endregion

            #region Compression
            // ѹ��
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                // ������Ӷ���ѹ�����ͣ��������ݼ����Զ���ȡ���ŷ�ʽ
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                // ���ָ����MimeType��ʹ��ѹ������
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "application/json" });
            });
            // ��Բ�ͬ��ѹ�����ͣ����ö�Ӧ��ѹ������
            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                // ʹ�����ķ�ʽ����ѹ������һ����ѹ��Ч����õķ�ʽ
                options.Level = CompressionLevel.Fastest;

                // ������ѹ������
                // options.Level = CompressionLevel.NoCompression;

                // ��ʹ��Ҫ�ķѺܳ���ʱ�䣬ҲҪʹ��ѹ��Ч����õķ�ʽ
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

            // ���ÿ������ ��������λ�ã�appִ�����Ⱥ�˳��
            app.UseCors("any");// ����Ҫ��һ���Ǿ��������м���ڹܵ��е�λ�ã�һ��Ҫ��������UseMvc()����֮ǰ
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}