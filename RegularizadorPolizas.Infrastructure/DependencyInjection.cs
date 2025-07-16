using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;
using RegularizadorPolizas.Infrastructure.Data;
using RegularizadorPolizas.Infrastructure.Data.Repositories;
using RegularizadorPolizas.Infrastructure.External;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;
using RegularizadorPolizas.Infrastructure.Repositories;
using RegularizadorPolizas.Infrastructure.Services;

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
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IBrokerRepository, BrokerRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            services.AddScoped<IVerificationRepository, VerificationRepository>();
            services.AddScoped<IAzureDocumentIntelligenceService, AzureDocumentIntelligenceService>();
            services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            services.AddScoped<IVelneoApiService, TenantAwareVelneoApiService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<SmartDocumentParser>();

            services.AddSingleton(provider =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Azure Storage connection string is not configured");
                }
                return new BlobServiceClient(connectionString);
            });

            services.AddHttpClient<TenantAwareVelneoApiService>((serviceProvider, client) =>
            {
                var baseUrl = configuration["VelneoAPI:BaseUrl"];
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }

                var timeoutSeconds = configuration.GetValue<int>("VelneoAPI:TimeoutSeconds", 120); 
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

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