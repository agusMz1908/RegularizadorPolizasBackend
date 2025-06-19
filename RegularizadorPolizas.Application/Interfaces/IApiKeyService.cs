using System.Threading.Tasks;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IApiKeyService
    {
        Task<ApiKey?> GetApiKeyAsync(string key);
    }
}