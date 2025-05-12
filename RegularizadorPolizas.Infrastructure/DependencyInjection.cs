using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;
using System;

namespace RegularizadorPolizas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IPolizaRepository, PolizaRepository>();
            services.AddScoped<IProcessDocumentRepository, ProcessDocumentRepository>();
            services.AddScoped<IRenovacionRepository, RenovationRepository>();

            // External services
            services.AddScoped<IAzureDocumentIntelligenceService, AzureDocumentIntelligenceService>();
            services.AddScoped<IVelneoApiService, VelneoApiService>();

            // HttpClient configuration for Velneo API
            services.AddHttpClient<IVelneoApiService, VelneoApiService>(client =>
            {
                client.BaseAddress = new Uri(configuration["VelneoAPI:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}