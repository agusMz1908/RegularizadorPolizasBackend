using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Configuration; 
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;
using RegularizadorPolizas.Infrastructure.Repositories;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 28)),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    )
                ));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IPolizaRepository, PolizaRepository>();
            services.AddScoped<IProcessDocumentRepository, ProcessDocumentRepository>();
            services.AddScoped<IRenovationRepository, RenovationRepository>();
            services.AddScoped<IBrokerRepository, BrokerRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IGenericRepository<Role>, GenericRepository<Role>>();
            services.AddScoped<IGenericRepository<UserRole>, GenericRepository<UserRole>>();
            services.AddScoped<IGenericRepository<Permission>, GenericRepository<Permission>>();
            services.AddScoped<IGenericRepository<RolePermission>, GenericRepository<RolePermission>>();

            services.AddScoped<IAzureDocumentIntelligenceService, AzureDocumentIntelligenceService>();

            services.AddHttpClient<IVelneoApiService, VelneoApiService>((serviceProvider, client) =>
            {
                // Configuración desde appsettings (como antes)
                var baseUrl = configuration["VelneoAPI:BaseUrl"];
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }

                // Timeout desde configuración o default
                var timeoutSeconds = configuration.GetValue<int>("VelneoAPI:TimeoutSeconds", 30);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

                // Headers adicionales
                var apiKey = configuration["VelneoAPI:ApiKey"];
                if (!string.IsNullOrEmpty(apiKey))
                {
                    client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                }

                client.DefaultRequestHeaders.Add("User-Agent", "RegularizadorPolizas-API/1.0");

                var apiVersion = configuration["VelneoAPI:ApiVersion"];
                if (!string.IsNullOrEmpty(apiVersion))
                {
                    client.DefaultRequestHeaders.Add("Api-Version", apiVersion);
                }
            });

            return services;
        }
    }
}