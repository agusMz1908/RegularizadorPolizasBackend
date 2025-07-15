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
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

Console.WriteLine("🔐 Configurando JWT Authentication...");
Console.WriteLine($"🔐 JWT Issuer: {jwtSettings["Issuer"]}");
Console.WriteLine($"🔐 JWT Audience: {jwtSettings["Audience"]}");
Console.WriteLine($"🔐 JWT Key configurado: {!string.IsNullOrEmpty(jwtSettings["Key"])}");
Console.WriteLine($"🔐 JWT Key length: {jwtSettings["Key"]?.Length} chars");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero 
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"🚨 JWT Authentication Failed: {context.Exception.Message}");
            Console.WriteLine($"🚨 Request Path: {context.Request.Path}");
            Console.WriteLine($"🚨 Token: {context.Request.Headers.Authorization.FirstOrDefault()}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var userName = context.Principal?.Identity?.Name ?? "Unknown";
            var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            Console.WriteLine($"✅ JWT Token Validated - User: {userName} (ID: {userId})");
            Console.WriteLine($"✅ Request Path: {context.Request.Path}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"⚠️ JWT Challenge triggered");
            Console.WriteLine($"⚠️ Error: {context.Error}");
            Console.WriteLine($"⚠️ Error Description: {context.ErrorDescription}");
            Console.WriteLine($"⚠️ Request Path: {context.Request.Path}");
            Console.WriteLine($"⚠️ Auth Header: {context.Request.Headers.Authorization.FirstOrDefault()}");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"📨 JWT Message Received for: {context.Request.Path}");
                Console.WriteLine($"📨 Token starts with: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
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
app.UseAuthentication();
app.UseAuthorization();
app.UseAuditMiddleware();
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
            JwtKeyLength = config["Jwt:Key"]?.Length,
            Timestamp = DateTime.UtcNow
        };
        return Results.Json(settings);
    }).AllowAnonymous();

    app.MapGet("/debug-jwt", (HttpContext context) =>
    {
        return Results.Ok(new
        {
            IsAuthenticated = context.User.Identity?.IsAuthenticated,
            UserName = context.User.Identity?.Name,
            Claims = context.User.Claims.Select(c => new { c.Type, c.Value }),
            JwtKeyConfigured = !string.IsNullOrEmpty(context.RequestServices.GetService<IConfiguration>()?["Jwt:Key"]),
            AuthHeader = context.Request.Headers.Authorization.FirstOrDefault()
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

    app.MapGet("/test-auth", (HttpContext context) =>
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            return Results.Ok(new
            {
                Message = "🎉 JWT Authentication is working!",
                User = context.User.Identity.Name,
                UserId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Tenant = context.User.FindFirst("tenant")?.Value,
                Roles = context.User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value),
                Timestamp = DateTime.UtcNow
            });
        }
        else
        {
            return Results.Unauthorized();
        }
    }).RequireAuthorization();
}
#endregion

Console.WriteLine("🚀 RegularizadorPolizas API iniciando...");
Console.WriteLine($"🌍 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔐 JWT habilitado: {!string.IsNullOrEmpty(builder.Configuration["Jwt:Key"])}");

app.Run();