using Microsoft.EntityFrameworkCore;

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
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
            // not recommended for production - you need to store your key material somewhere secure
            .AddDeveloperSigningCredential()
            .AddTestUsers(TestUsers.Users)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryIdentityResources(Config.IdentityResources)
            // .AddConfigurationStore(opt =>
            // {
            //     opt.ConfigureDbContext = c => c.UseSqlServer(
            //         configuration.GetConnectionString("IdentitySqlConnection"),
            //         builder => builder.MigrationsAssembly("TeduMicroservices.IDP"));
            // })
            // .AddOperationalStore(opt =>
            // {
            //     opt.ConfigureDbContext = c => c.UseSqlServer(
            //         configuration.GetConnectionString("IdentitySqlConnection"),
            //         builder => builder.MigrationsAssembly("TeduMicroservices.IDP"));
            // })
            ;
    }
}