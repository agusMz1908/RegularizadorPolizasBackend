using System.Net.Http;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IVelneoHttpService
    {
        Task<HttpClient> GetConfiguredHttpClientAsync();
        Task<string> BuildVelneoUrlAsync(string endpoint);
        Task<IEnumerable<TEntity>> DeserializeWithFallbackAsync<TResponse, TEntity>(
            HttpResponseMessage response,
            Func<TResponse, IEnumerable<TEntity>?> extractFromWrapper,
            string entityName)
            where TResponse : class
            where TEntity : class;
        Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response) where T : class;
    }
}