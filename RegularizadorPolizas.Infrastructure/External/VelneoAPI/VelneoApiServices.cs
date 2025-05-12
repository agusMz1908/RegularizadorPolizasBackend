using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.DTOs;
using System.Net.Http.Headers;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI
{
    public class VelneoApiService : IVelneoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public VelneoApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? new HttpClient(); // Usa un cliente por defecto si no se proporciona uno
            _baseUrl = configuration?["VelneoAPI:BaseUrl"] ?? "https://api.velneo.com"; // Valor por defecto
            _apiKey = configuration?["VelneoAPI:ApiKey"] ?? "your-api-key-here"; // Valor por defecto

            if (_httpClient.DefaultRequestHeaders.Accept.Count == 0)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<ClientDto> GetClientAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/clientes/{id}?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClientResponse>(content);

            if (result.Clients.Count > 0)
            {
                return result.Clients[0];
            }

            return null;
        }

        public async Task<PolizaDto> GetPolizaAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/contratos/{id}?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PolizaResponse>(content);

            if (result.Polizas.Count > 0)
            {
                return result.Polizas[0];
            }

            return null;
        }

        public async Task<UserDto> GetUsersAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/usu/{id}?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserResponse>(content);

            if (result.Users.Count > 0)
            {
                return result.Users[0];
            }

            return null;
        }

        private class ClientResponse
        {
            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("total_count")]
            public int TotalCount { get; set; }

            [JsonProperty("clientes")]
            public List<ClientDto> Clients { get; set; }
        }

        private class PolizaResponse
        {
            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("total_count")]
            public int TotalCount { get; set; }

            [JsonProperty("contratos")]
            public List<PolizaDto> Polizas { get; set; }
        }

        private class UserResponse
        {
            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("total_count")]
            public int TotalCount { get; set; }

            [JsonProperty("usu")]
            public List<UserDto> Users { get; set; }
        }
    }
}