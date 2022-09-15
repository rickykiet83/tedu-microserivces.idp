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
    internal static void AddAppConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
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
            endpoints.MapDefaultControllerRoute().RequireAuthorization("Bearer");
            endpoints.MapRazorPages().RequireAuthorization();
        });

        return app;
    }
}
