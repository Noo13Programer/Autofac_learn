using Autofac;
using Autofac.Core;
using IdentityServer4_Application.IdentityResource;
using IdentityServer4_Application_Contracts.IdentityResource;
using IdentityServer4_Domain.LifeTime;
using System.Reflection;

namespace IdentityServer4_WebApi.AutoFac
{
    public class AutofacModule:Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var applicationAppAssembly = Assembly.Load("IdentityServer4_Application");
            var applicationContractsAppAssembly = Assembly.Load("IdentityServer4_Application_Contracts");
            var domainAppAssembly = Assembly.Load("IdentityServer4_Domain");

            var assemblies = Assembly.GetExecutingAssembly();
            //var referenceAssemblies = assemblies.GetReferencedAssemblies().Select(Assembly.Load).ToArray();
            var allAssenblies = new[] {assemblies, applicationAppAssembly, applicationContractsAppAssembly, domainAppAssembly }.ToArray();

            //builder.RegisterType<IdentityResourceAppService>().As<IIdentityResourceAppService>();

            // 注册所有名称以 "AppService" 结尾的公共类，并将它们注册为它们实现的所有接口
            builder.RegisterAssemblyTypes(allAssenblies)
                   .Where(c => c.Name.EndsWith("AppService"))
                   .PublicOnly()
                   .Where(cc => cc.IsClass)
                   .AsImplementedInterfaces();


            // 注册所有实现了 ITransientDependency 接口的具体类为瞬态服务
            builder.RegisterAssemblyTypes(allAssenblies)
                   .Where(t => typeof(ITransientDependency).IsAssignableFrom(t) && t.IsClass)
                   .As<ITransientDependency>()
                   .InstancePerDependency();

            // 注册所有实现了 ISingletonDependency 接口的具体类为单例服务
            builder.RegisterAssemblyTypes(allAssenblies)
                   .Where(t => typeof(ISingletonDependency).IsAssignableFrom(t) && t.IsClass)
                   .As<ISingletonDependency>()
                   .SingleInstance();

            // 注册所有实现了 IScopeDependency 接口的具体类为作用域服务
            builder.RegisterAssemblyTypes(allAssenblies)
                   .Where(t => typeof(IScopeDependency).IsAssignableFrom(t) && t.IsClass)
                   .As<IScopeDependency>()
                   .InstancePerLifetimeScope();

            foreach (var assembly in allAssenblies)
            {
                Console.WriteLine($"Scanning assembly: {assembly.FullName}");
                var types = assembly.GetTypes().Where(t => t.Name.EndsWith("AppService"));
                foreach (var type in types)
                {
                    Console.WriteLine($"Found type: {type.FullName}");
                }
            }
        }
    }
}
