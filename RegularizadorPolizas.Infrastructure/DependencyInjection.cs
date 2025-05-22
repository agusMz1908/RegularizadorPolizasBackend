using Microsoft.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;

namespace RegularizadorPolizas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    )
                ));

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IPolizaRepository, PolizaRepository>();
            services.AddScoped<IProcessDocumentRepository, ProcessDocumentRepository>();
            services.AddScoped<IRenovationRepository, RenovationRepository>();

            services.AddScoped<IAzureDocumentIntelligenceService, AzureDocumentIntelligenceService>();

            services.AddHttpClient<IVelneoApiService, VelneoApiService>(client =>
            {
                var baseUrl = configuration["VelneoAPI:BaseUrl"];
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }

                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}