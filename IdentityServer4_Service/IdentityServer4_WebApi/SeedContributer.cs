using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4_WebApi
{
    public static class SeedContributer
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("identityApi", "认证中心第一个Api")
             };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
             {
                 new Client
                {
                  ClientId = "identity-service",
                  Description ="客户端凭证",
                  AllowedGrantTypes = GrantTypes.ClientCredentials,
                  ClientSecrets =
                {
                    new Secret("123456".Sha256())
                 },
    
                     // scopes that client has access to
                     AllowedScopes = { "ids4-api" }
                  }
                 };
        }

        public static IEnumerable<ApiResource> GetResources()
        {
            return new ApiResource[]
            {
                new ApiResource
                {
                    Name="ids4-resource",
                    Description="ids4认证",
                    ApiSecrets = {new Secret("123456".Sha256()) },
                    Scopes = new List<string>
                    {
                        "ids4-api"
                    }
                }
            };
        }

        public static IEnumerable<ApiScope> GetScopes()
        {
            return new List<ApiScope>
            {

                new ApiScope("ids4-api","ids4范围")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }



        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in GetScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
