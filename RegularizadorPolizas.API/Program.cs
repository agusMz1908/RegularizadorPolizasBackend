using Microsoft.EntityFrameworkCore;
using RegularizadorPolizas.Application;
using RegularizadorPolizas.Infrastructure;
using RegularizadorPolizas.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics;
using RegularizadorPolizas.API.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<RegularizadorPolizas.Application.Interfaces.IApiKeyService, RegularizadorPolizas.Application.Services.ApiKeyService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RegularizadorPolizas API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin() 
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
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
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.Map("/error", (HttpContext context) =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error;

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    logger.LogError(exception, "An unhandled exception occurred during request processing.");

    var errorResponse = new
    {
        StatusCode = StatusCodes.Status500InternalServerError,
        Message = "An unexpected error occurred. Please try again later.",
        ErrorDetails = app.Environment.IsDevelopment() ? exception?.Message : null
    };

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json";

    return Results.Json(errorResponse);
});

app.UseHttpsRedirection(); 
app.UseCors("AllowAll");
app.UseSession();
app.UseAuditMiddleware();
app.UseAuthentication();  
app.UseAuthorization();   
app.UseMiddleware<RegularizadorPolizas.API.Middleware.ApiKeyMiddleware>();

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