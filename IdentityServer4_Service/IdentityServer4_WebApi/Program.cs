
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4_WebApi.AutoFac;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

namespace IdentityServer4_WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            builder.WebHost.UseUrls(configuration["BaseUrl"]?.Split(","));

            // Add services to the container.
            ConfigureServiceProvider(builder.Host);


            var connectStr = builder.Configuration.GetConnectionString("default");
            ConfiguraService(builder.Services, connectStr);

            


            var app = builder.Build();

            // 从服务提供者中获取 Autofac 容器
            //var container = app.Services;

            //if (container != null)
            //{
            //    // 获取所有注册的服务
            //    var registeredServices = container.ComponentRegistry.Registrations.SelectMany(r => r.Services);

            //    Console.WriteLine("已注册的服务列表：");
            //    foreach (var service in registeredServices)
            //    {
            //        Console.WriteLine(service.Description);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("无法获取 Autofac 容器实例。");
            //}

            SeedContributer.InitializeDatabase(app);
            var pathBase = configuration["PathBase"];
            // 配置路径基础
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(new PathString(pathBase));
            }
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService V1");
                    c.RoutePrefix = "swagger";
                });
            }

            //app.UseHttpsRedirection();

            app.UseCookiePolicy();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseCors("default");
            app.UseIdentityServer();
            app.UseAuthorization();


            app.MapControllers();

           

            app.Run();
        }

        private static void ConfigureServiceProvider(ConfigureHostBuilder host)
        {
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule<AutofacModule>();
            });
        }

        
        private static void ConfiguraService(IServiceCollection services, string? connectStr)
        {
            services.AddControllers();
            services.ConfigureNonBreakingSameSiteCookies();
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
            // in DB  config
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddConfigurationStore(options => //添加配置数据（ConfigurationDbContext上下文用户配置数据）
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectStr, sql => sql.MigrationsAssembly(migrationsAssembly));
            }).AddOperationalStore(options =>   //添加操作数据（PersistedGrantDbContext上下文 临时数据（如授权和刷新令牌））
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectStr, sql => sql.MigrationsAssembly(migrationsAssembly));
                // 自动清理 token ，可选
                options.EnableTokenCleanup = true;
                // 自动清理 token ，可选
                options.TokenCleanupInterval = 30;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}
