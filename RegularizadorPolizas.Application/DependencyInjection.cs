using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Configuration;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;
using System.Reflection;

namespace RegularizadorPolizas.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IPolizaService, PolizaService>();
            services.AddScoped<IProcessDocumentService, ProcessDocumentService>();
            services.AddScoped<IRenovationService, RenovationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDocumentValidationService, DocumentValidationService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IBrokerService, BrokerService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IHybridApiService, BusinessSpecificHybridService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();

            services.Configure<HybridApiConfiguration>(config =>
            {
                config.EnableLocalAudit = true;
                config.EnableVelneoFallback = true;
                config.EnableLocalCaching = false;

                if (!config.EntityRouting.Any())
                {
                    var entities = new[] { "Client", "Broker", "Currency", "Company", "Poliza" };
                    var operations = new[] { "GET", "CREATE", "UPDATE", "DELETE", "SEARCH" };

                    foreach (var entity in entities)
                    {
                        foreach (var operation in operations)
                        {
                            if (entity == "Currency")
                            {
                                config.EntityRouting[$"{entity}.{operation}"] = "Velneo";
                            }
                            else
                            {
                                config.EntityRouting[$"{entity}.{operation}"] = "Local";
                            }
                        }
                    }

                    config.EntityRouting["Document.PROCESS"] = "Local";
                    config.EntityRouting["Document.EXTRACT"] = "Local";
                    config.EntityRouting["Document.CREATE_POLIZA"] = "Local";
                }
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}