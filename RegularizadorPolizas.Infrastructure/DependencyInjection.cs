using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;

namespace RegularizadorPolizas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IPolizaRepository, PolizaRepository>();
            services.AddScoped<IProcessDocumentRepository, ProcessDocumentRepository>();
            services.AddScoped<IRenovationRepository, RenovationRepository>();

            services.AddScoped<IAzureDocumentIntelligenceService, AzureDocumentIntelligenceService>();
            services.AddScoped<IVelneoApiService, VelneoApiService>();

            return services;
        }
    }
}
