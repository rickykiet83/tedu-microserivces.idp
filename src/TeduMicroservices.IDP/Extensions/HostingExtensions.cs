using Microsoft.Extensions.Configuration.AzureKeyVault;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TeduMicroservices.IDP.Infrastructure.Domains;
using TeduMicroservices.IDP.Infrastructure.Repositories;
using TeduMicroservices.IDP.Presentation;
using TeduMicroservices.IDP.Services.EmailService;

namespace TeduMicroservices.IDP.Extensions;

internal static class HostingExtensions
{
    internal static void AddAppConfigurations(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    private static void AddAzureKeyVaultManagedIdentityClientId(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var keyVaultName = configuration["Azure:KeyVaultName"];
        if (string.IsNullOrEmpty(keyVaultName)) return;

        var keyVaultUrl = new Uri($"https://{keyVaultName}.vault.azure.net/").ToString();
        var clientId = configuration["Azure:ClientId"];
        var clientSecret = configuration["Azure:ClientSecret"];

        builder.Configuration.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret,
            new DefaultKeyVaultSecretManager());

        var host = configuration.GetValue<string>("ConnectionStrings:Host");
        var database = configuration.GetValue<string>("ConnectionStrings:Database");
        var userId = configuration.GetValue<string>("ConnectionStrings:UserId");
        var dbPasswordValue = configuration["db-password"]; //secret name in Azure Key Vault
        if (string.IsNullOrEmpty(dbPasswordValue))
            throw new ArgumentNullException("db-password is not configured in Azure Key Vault.");

        var connectionString =
            $"Server={host},1433;Initial Catalog={database};Persist Security Info=False;User ID={userId};Password={dbPasswordValue};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "ConnectionStrings:IdentitySqlConnection", connectionString }
        });
    }

    public static void ConfigureSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
        {
            var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            var username = context.Configuration.GetValue<string>("ElasticConfiguration:Username");
            var password = context.Configuration.GetValue<string>("ElasticConfiguration:Password");
            var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");

            if (string.IsNullOrEmpty(elasticUri))
                throw new Exception("ElasticConfiguration Uri is not configured.");

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console().ReadFrom.Configuration(context.Configuration)
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        IndexFormat =
                            $"{applicationName}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfShards = 2,
                        NumberOfReplicas = 1,
                        ModifyConnectionSettings = x => x.BasicAuthentication(username, password)
                    })
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Application", applicationName)
                .ReadFrom.Configuration(context.Configuration);
        });
    }

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.AddAzureKeyVaultManagedIdentityClientId();
        builder.Services.AddControllersWithViews();
        builder.Services.AddConfigurationSettings(builder.Configuration);
        builder.Services.AddAutoMapper(typeof(Program));
        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();

        // Add services to the container
        builder.Services.AddScoped<IEmailSender, SmtpMailService>();
        builder.Services.ConfigureCookiePolicy();
        builder.Services.ConfigureCors();
        builder.Services.ConfigureIdentity(builder.Configuration);
        builder.Services.ConfigureIdentityServer(builder.Configuration);
        builder.Services.AddTransient(typeof(IUnitOfWork),
            typeof(UnitOfWork));
        builder.Services.AddTransient(typeof(IRepositoryBase<,>),
            typeof(RepositoryBase<,>));
        builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
        builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
        builder.Services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;
            config.Filters.Add(new ProducesAttribute("application/json", "text/plain", "text/json"));
        }).AddApplicationPart(typeof(AssemblyReference).Assembly);
        builder.Services.ConfigureAuthentication();
        builder.Services.ConfigureAuthorization();
        builder.Services.ConfigureSwagger(builder.Configuration);
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        builder.Services.ConfigureHealthChecks(builder.Configuration);
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseCors("CorsPolicy");
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.OAuthClientId("tedu_microservices_swagger");
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tedu Identity API");
            c.DisplayRequestDuration();
        });
        app.UseRouting();
        app.UseMiddleware<ErrorWrappingMiddleware>();
        //set cookie policy before authentication/authorization setup
        app.UseCookiePolicy();
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapDefaultControllerRoute().RequireAuthorization("Bearer");
            endpoints.MapRazorPages().RequireAuthorization();
        });

        return app;
    }
}