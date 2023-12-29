using Autofac;
using Autofac.Extras.DynamicProxy;
using System.Reflection;

namespace WebAPI.Net6.BaseRepositoryGenerics.Extensions
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 服务项目程序集
            Assembly shoppingServiceAssembly = Assembly.Load("CWT.BookingEngine.ShoppingService");

            // 注册UnitOfWork
            builder.RegisterAssemblyTypes(shoppingServiceAssembly)
                .Where(n => n.FullName.EndsWith("UnitOfWork") && !n.IsAbstract)
                .InstancePerLifetimeScope() // 瞬时单例 .InstancePerDependency()
                .AsImplementedInterfaces() // 自动以其实现的所有接口类型暴露（包括IDisposable接口）
                .EnableInterfaceInterceptors(); // 引用Autofac.Extras.DynamicProxy;

            // 注册Repository
            builder.RegisterAssemblyTypes(shoppingServiceAssembly)
                .Where(n => n.FullName.EndsWith("Repository") && !n.IsAbstract)
                .InstancePerLifetimeScope() // 瞬时单例 .InstancePerDependency()
                .AsImplementedInterfaces() // 自动以其实现的所有接口类型暴露（包括IDisposable接口）
                .EnableInterfaceInterceptors(); // 引用Autofac.Extras.DynamicProxy;

            // 注册Service
            builder.RegisterAssemblyTypes(shoppingServiceAssembly)
                .Where(n => n.FullName.EndsWith("Service") && !n.IsAbstract)
                .InstancePerLifetimeScope() // 瞬时单例 .InstancePerDependency()
                .AsImplementedInterfaces() // 自动以其实现的所有接口类型暴露（包括IDisposable接口）;
                .EnableInterfaceInterceptors(); // 引用Autofac.Extras.DynamicProxy;
        }
    }
}
