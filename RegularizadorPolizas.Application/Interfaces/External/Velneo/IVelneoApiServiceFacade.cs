using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.Application.Interfaces.External
{
    public interface IVelneoApiServiceFacade : IVelneoApiService
    {
        // ✅ Esta interfaz hereda TODOS los métodos de IVelneoApiService
        // No necesita declarar métodos adicionales porque el facade
        // implementará IVelneoApiService delegando a servicios especializados
    }

    public class FacadeMetricsDto
    {
        public int TotalMethodsCalled { get; set; }
        public int ClientServiceCalls { get; set; }
        public int CompanyServiceCalls { get; set; }
        public int PolizaServiceCalls { get; set; }
        public int MaestrosServiceCalls { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public DateTime LastResetTime { get; set; }
    }
    public class HealthCheckDto
    {
        public bool ClientServiceHealthy { get; set; }
        public bool CompanyServiceHealthy { get; set; }
        public bool PolizaServiceHealthy { get; set; }
        public bool MaestrosServiceHealthy { get; set; }
        public bool HttpServiceHealthy { get; set; }
        public string? ErrorDetails { get; set; }
    }
}