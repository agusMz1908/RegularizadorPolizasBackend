using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using RegularizadorPolizas.Application;
using RegularizadorPolizas.Infrastructure;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.API.Middleware;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;
using RegularizadorPolizas.Application.Mappings;
using RegularizadorPolizas.Infrastructure.Data.Repositories;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;
using RegularizadorPolizas.Application.Services.External;

var builder = WebApplication.CreateBuilder(args);

#region Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
#endregion

#region Core Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
#endregion

#region Multi-Tenant & API Key Services
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddScoped<IVelneoApiService, TenantAwareVelneoApiService>();

builder.Services.AddHttpClient();
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(ApiKeyMappingProfile), typeof(Program));
#endregion

#region Controllers & API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
#endregion

#region Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RegularizadorPolizas API",
        Version = "v1",
        Description = "API para gestión de pólizas de seguro con sistema multi-tenant"
    });

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

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Example: X-Api-Key: {your-api-key}",
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey
    });
});
#endregion

#region CORS Configuration
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Configuración permisiva para desarrollo
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:3001",
                    "https://localhost:3000",
                    "https://localhost:3001",
                    "http://localhost:5173",           
                    "https://localhost:5173"         
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    }
    else
    {
        var allowedOrigins = builder.Configuration.GetSection("Frontend:AllowedOrigins").Get<string[]>() ?? new string[0];

        options.AddPolicy("AllowAll", policy =>
        {
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        });
    }
});
#endregion

#region Session Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
#endregion

#region JWT Authentication
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero 
    };

    if (builder.Environment.IsDevelopment())
    {
        options.RequireHttpsMetadata = false;
    }
});
#endregion

var app = builder.Build();

#region Database Initialization
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
#endregion

#region Development Environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RegularizadorPolizas API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts(); 
}
#endregion

#region Error Handling
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
        ErrorDetails = app.Environment.IsDevelopment() ? exception?.Message : null,
        Timestamp = DateTime.UtcNow
    };

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json";

    return Results.Json(errorResponse);
});
#endregion

#region Middleware Pipeline
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseSession();

app.UseAuditMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ApiKeyMiddleware>();
#endregion

#region Controllers
app.MapControllers();
#endregion

#region Development Endpoints
if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug-config", (IConfiguration config) =>
    {
        var settings = new
        {
            Environment = app.Environment.EnvironmentName,
            AzureEndpoint = config["AzureDocumentIntelligence:Endpoint"],
            VelneoUrl = config["VelneoAPI:BaseUrl"],
            ConnectionStringConfigured = !string.IsNullOrEmpty(config.GetConnectionString("DefaultConnection")),
            JwtIssuer = config["Jwt:Issuer"],
            JwtAudience = config["Jwt:Audience"],
            JwtKeyConfigured = !string.IsNullOrEmpty(config["Jwt:Key"]),
            Timestamp = DateTime.UtcNow
        };
        return Results.Json(settings);
    }).AllowAnonymous();

    app.MapGet("/debug-jwt", (HttpContext context) =>
    {
        return Results.Ok(new
        {
            IsAuthenticated = context.User.Identity?.IsAuthenticated,
            Claims = context.User.Claims.Select(c => new { c.Type, c.Value }),
            JwtKeyConfigured = !string.IsNullOrEmpty(context.RequestServices.GetService<IConfiguration>()?["Jwt:Key"])
        });
    }).RequireAuthorization();

    app.MapGet("/health", () =>
    {
        return Results.Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = app.Environment.EnvironmentName
        });
    }).AllowAnonymous();
}
#endregion

app.Run();