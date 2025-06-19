using System.Threading.Tasks;
using RegularizadorPolizas.Domain.Entities;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByKeyAsync(string key);
    Task AddAsync(ApiKey apiKey);
    Task DeleteAsync(int id);
    Task<IEnumerable<ApiKey>> GetAllAsync();
}