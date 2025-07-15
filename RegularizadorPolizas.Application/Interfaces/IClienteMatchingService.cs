using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IClienteMatchingService
    {
        Task<ClienteMatchResult> BuscarClienteAsync(DatosClienteExtraidos datosExtraidos);
        Task<ClienteMatchResult> BuscarClientePorDocumentoAsync(string documento);
        Task<ClienteMatchResult> BuscarClientePorNombreAsync(string nombre);
        Task<List<ClienteMatch>> BuscarClientesAvanzadoAsync(DatosClienteExtraidos datos, int limiteSugerencias = 10);
    }
}