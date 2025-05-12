using Microsoft.Extensions.DependencyInjection;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services;
using System.Reflection;

namespace RegularizadorPolizas.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IPolizaService, PolizaService>();
            services.AddScoped<IProcessDocumentService, ProcessDocumentService>();
            services.AddScoped<IRenovationService, RenovationService>();
            services.AddScoped<IAuthService, AuthService>();

            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}