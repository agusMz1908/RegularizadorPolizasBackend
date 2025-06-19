using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace RegularizadorPolizas.API.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "X-Api-Key";
        private const string APIKEYQUERY = "api_key";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IApiKeyService apiKeyService)
        {
            string? extractedApiKey = null;

            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (
                path.StartsWith("/api/auth/login") ||
                path.StartsWith("/api/auth/validate-token") ||
                path.StartsWith("/swagger")
            ))
            {
                await _next(context);
                return;
            }

            if (context.Request.Headers.TryGetValue(APIKEYNAME, out var headerApiKey))
            {
                extractedApiKey = headerApiKey.FirstOrDefault();
            }

            else if (context.Request.Query.TryGetValue(APIKEYQUERY, out var queryApiKey))
            {
                extractedApiKey = queryApiKey.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            var apiKey = await apiKeyService.GetApiKeyAsync(extractedApiKey);
            if (apiKey == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or expired API Key");
                return;
            }

            context.Items["TenantId"] = apiKey.TenantId;
            await _next(context);
        }
    }
} 