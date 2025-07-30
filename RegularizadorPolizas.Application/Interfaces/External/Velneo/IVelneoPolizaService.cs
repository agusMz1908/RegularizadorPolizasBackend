using RegularizadorPolizas.Application.DTOs;

public interface IVelneoPolizaService
{
    Task<PolizaDto> GetPolizaAsync(int id);
    Task<IEnumerable<PolizaDto>> GetPolizasAsync();
    Task<IEnumerable<PolizaDto>> GetPolizasByClientAsync(int clienteId);
    Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasByClientPaginatedAsync(
        int clienteId,
        int page = 1,
        int pageSize = 25,
        string? search = null);
    Task<PaginatedVelneoResponse<PolizaDto>> GetPolizasPaginatedAsync(
        int page = 1,
        int pageSize = 50,
        string? search = null);
    Task<object> CreatePolizaFromRequestAsync(PolizaCreateRequest request);
}