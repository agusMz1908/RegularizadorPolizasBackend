using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;
using RegularizadorPolizas.Infrastructure.Services;
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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();

            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IHybridApiService, TenantAwareHybridService>();

            services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            services.AddScoped<IVerificationService, VerificationService>();
            services.AddScoped<IVerificationService, VerificationService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}