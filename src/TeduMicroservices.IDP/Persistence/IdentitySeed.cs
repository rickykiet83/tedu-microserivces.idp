using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace TeduMicroservices.IDP.Persistence;

public static class IdentitySeed
{
    public static async Task<IHost> MigrateDatabaseAsync(this IHost host, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        using var scope = host.Services.CreateScope();

        await using var context = scope.ServiceProvider
            .GetRequiredService<ConfigurationDbContext>();
        context.Database.SetConnectionString(connectionString);
        await context.Database.MigrateAsync();

        await using var persistedGrantDbContext = scope.ServiceProvider
            .GetRequiredService<PersistedGrantDbContext>();
        persistedGrantDbContext.Database.SetConnectionString(connectionString);
        await persistedGrantDbContext.Database.MigrateAsync();

        await using var teduContext = scope.ServiceProvider
           .GetRequiredService<TeduIdentityContext>();
        teduContext.Database.SetConnectionString(connectionString);
        await teduContext.Database.MigrateAsync();
        try
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var apiScope in Config.ApiScopes)
                {
                    context.ApiScopes.Add(apiScope.ToEntity());
                }
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
            }

            await context.SaveChangesAsync();
            await teduContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return host;
    }
}