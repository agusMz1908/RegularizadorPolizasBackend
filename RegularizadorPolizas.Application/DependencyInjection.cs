using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;

namespace RegularizadorPolizas.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar servicios
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IPolizaService, PolizaService>();
            services.AddScoped<IProcessDocumentRepository, ProcessDocumentService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}