using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RegularizadorPolizas.API.Middleware;
using RegularizadorPolizas.Application;
using RegularizadorPolizas.Application.External.Velneo;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using RegularizadorPolizas.Application.Mappings;
using RegularizadorPolizas.Application.Services;
using RegularizadorPolizas.Infrastructure;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Services;
using System.Text;

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
builder.Services.AddScoped<IVelneoHttpService, VelneoHttpService>();

builder.Services.AddScoped<IVelneoMaestrosService, VelneoMaestrosService>();

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
        Description = "JWT Authorization header using the Bearer scheme. " +
                      "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                      "Example: \"Bearer 12345abcdef\"",
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
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
#endregion

#region JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];       
var jwtIssuer = builder.Configuration["Jwt:Issuer"]; 
var jwtAudience = builder.Configuration["Jwt:Audience"]; 

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured. Please check your appsettings.json or user secrets.");
}

Console.WriteLine($"✅ JWT Configuration loaded - Issuer: {jwtIssuer}, Audience: {jwtAudience}");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var userName = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
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

    return Results.Problem(
        detail: app.Environment.IsDevelopment() ? exception?.ToString() : "An error occurred while processing your request.",
        statusCode: 500,
        title: "Internal Server Error"
    );
});
#endregion

#region CORS
app.UseCors(builder =>
{
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});
#endregion

#region Middleware Pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ApiKeyMiddleware>();
app.MapControllers();
#endregion

#region Startup Logging
app.Logger.LogInformation("🚀 RegularizadorPolizas API starting up...");
app.Logger.LogInformation("🔧 Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("🌐 CORS: Allow any origin");
app.Logger.LogInformation("🔐 JWT Authentication: Enabled");
app.Logger.LogInformation("📊 Swagger: Available at /swagger");

// ✅ ARQUITECTURA UNIFICADA COMPLETADA
app.Logger.LogInformation("🎯 VelneoMaestrosService: Unified architecture active");
app.Logger.LogInformation("📋 25 methods unified: Maestros + Clientes + Compañías");
app.Logger.LogInformation("🏗️ Refactoring completed: 3 services → 1 unified service");
#endregion

app.Run();