using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application;
using RegularizadorPolizas.Infrastructure;
using RegularizadorPolizas.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.CanConnect())
        {
            app.Logger.LogInformation("Database connection successful");
            context.Database.Migrate();
            app.Logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            app.Logger.LogError("Could not connect to the database");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug-config", (IConfiguration config) => {
        var settings = new
        {
            AzureEndpoint = config["AzureDocumentIntelligence:Endpoint"],
            VelneoUrl = config["VelneoAPI:BaseUrl"],
            ConnectionStringConfigured = !string.IsNullOrEmpty(config.GetConnectionString("DefaultConnection"))
        };
        return Results.Json(settings);
    });
}

app.Run();