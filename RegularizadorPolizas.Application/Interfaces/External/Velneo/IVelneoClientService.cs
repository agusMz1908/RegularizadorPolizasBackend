using RegularizadorPolizas.Application.DTOs;

public interface IVelneoClientService
{
    Task<ClientDto> GetClienteAsync(int id);
    Task<IEnumerable<ClientDto>> GetClientesAsync();

    Task<PaginatedVelneoResponse<ClientDto>> GetClientesPaginatedAsync(
        int page = 1,
        int pageSize = 50,
        string? search = null);

    Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm);
    Task<IEnumerable<ClientDto>> SearchClientesDirectAsync(string searchTerm);
}