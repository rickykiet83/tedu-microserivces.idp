using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using TeduMicroservices.IDP.Common;
using TeduMicroservices.IDP.Infrastructure.Entities;
using TeduMicroservices.IDP.Persistence;
namespace TeduMicroservices.IDP.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            .Get<SMTPEmailSetting>();
        services.AddSingleton(emailSettings);
        
        return services;
    }
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }
    
    public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var issuerUri = configuration.GetSection("IdentityServer:IssuerUri").Value;
        services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.IssuerUri = issuerUri;
            })
            // not recommended for production - you need to store your key material somewhere secure
            .AddDeveloperSigningCredential()
            .UseIdentityServerStoreConfig(configuration)
            .AddAspNetIdentity<User>()
            .AddProfileService<IdentityProfileService>()
            ;
    }

    private static IIdentityServerBuilder UseIdentityServerStoreConfig(this IIdentityServerBuilder builder, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        builder
            .AddConfigurationStore(opt =>
            {
                opt.ConfigureDbContext = c => c.UseSqlServer(
                    connectionString,
                    buider => buider.MigrationsAssembly("TeduMicroservices.IDP"));
            })
            .AddOperationalStore(opt =>
            {
                opt.ConfigureDbContext = c => c.UseSqlServer(
                    connectionString,
                    buider => buider.MigrationsAssembly("TeduMicroservices.IDP"));
            });
        return builder;
    }

    public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        services
            .AddDbContext<TeduIdentityContext>(options => options
                .UseSqlServer(connectionString))
            .AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddEntityFrameworkStores<TeduIdentityContext>()
            .AddUserStore<TeduUserStore>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Tedu Identity Server API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Tedu Identity Service",
                    Email = "kietpham.dev@gmail.com",
                    Url = new Uri("https://kietpham.dev")
                }
            });
            var identityServerBaseUrl = configuration.GetSection("IdentityServer:BaseUrl").Value;
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{identityServerBaseUrl}/connect/authorize"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "tedu_microservices_api.read", "Tedu Microservices API Read Scope" },
                            { "tedu_microservices_api.write", "Tedu Microservices API Write Scope" }
                        }
                    }
                }
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new List<string>
                    {
                        "tedu_microservices_api.read", 
                        "tedu_microservices_api.write"
                    }
                }
            });
        });
    }
    
    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication()
            .AddLocalApi("Bearer", option =>
            {
                option.ExpectedScope = "tedu_microservices_api.read";
            });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(
            options =>
            {
                options.AddPolicy("Bearer", policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer");
                    policy.RequireAuthenticatedUser();
                });
            });
    }

    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        services.AddHealthChecks()
            .AddSqlServer(connectionString,
                name: "SqlServer Health",
                failureStatus: HealthStatus.Degraded);
    }
}