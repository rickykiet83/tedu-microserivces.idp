using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.EntityFramework.DbContexts; 
using Duende.IdentityServer.EntityFramework.Mappers;
namespace TeduMicroservices.IDP.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });
    }
    
    public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("IdentitySqlConnection");
        services.AddIdentityServer(
                options =>
                {
                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = true;
                })
            // not recommended for production - you need to store your key material somewhere secure
            .AddDeveloperSigningCredential()
            .AddTestUsers(TestUsers.Users)
            .AddConfigurationStore(opt =>
            {
                opt.ConfigureDbContext = c => c.UseSqlServer(
                    configuration.GetConnectionString("IdentitySqlConnection"),
                    builder => builder.MigrationsAssembly("TeduMicroservices.IDP"));
            })
            .AddOperationalStore(opt =>
            {
                opt.ConfigureDbContext = c => c.UseSqlServer(
                    configuration.GetConnectionString("IdentitySqlConnection"),
                    builder => builder.MigrationsAssembly("TeduMicroservices.IDP"));
            });
    }
}