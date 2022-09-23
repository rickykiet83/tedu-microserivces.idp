using Serilog;
using TeduMicroservices.IDP.Extensions;
using TeduMicroservices.IDP.Persistence;

Log.Information("Starting up");
var builder = WebApplication.CreateBuilder(args);
try
{
    builder.Host.AddAppConfigurations();
    builder.Host.ConfigureSerilog();
    
    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    if (app.Environment.IsDevelopment())
    {
        SeedUserData.EnsureSeedData(builder.Configuration.GetConnectionString("IdentitySqlConnection"));
    }
    
    app.MigrateDatabase()
        .Run();
}

catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}