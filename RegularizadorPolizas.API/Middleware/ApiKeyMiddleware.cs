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
            var path = context.Request.Path.Value?.ToLower();

            var excludedPaths = new[]
            {
                "/api/auth",
                "/swagger",
                "/api/apikeys",
                "/api/users",
                "/api/roles",
                "/api/permissions",
                "/api/tenantswitch",
                "/api/companies",
                "/api/clients",
                "/api/clientes",
                "/api/currencies",
                "/api/brokers",
                "/api/polizas",
                "/error",
                "/debug-config",
                "/debug-jwt",
                "/test-auth",
                "/health",

                "/api/azure-document",         
                "/api/azuredocument"          
            };

            Console.WriteLine($"🔍 ApiKeyMiddleware - Path: {path}");
            var isExcluded = path != null && excludedPaths.Any(excluded => path.StartsWith(excluded));
            Console.WriteLine($"🔍 ApiKeyMiddleware - Is Excluded: {isExcluded}");

            if (isExcluded)
            {
                Console.WriteLine($"✅ ApiKeyMiddleware - Path {path} está excluido, pasando al siguiente middleware");
                await _next(context);
                return;
            }

            Console.WriteLine($"⚠️ ApiKeyMiddleware - Path {path} requiere API Key");

            string? extractedApiKey = null;

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
                Console.WriteLine($"❌ ApiKeyMiddleware - API Key missing for {path}");
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"API Key is missing\", \"hint\": \"Use X-Api-Key header or api_key query parameter\"}");
                return;
            }

            var apiKey = await apiKeyService.GetApiKeyAsync(extractedApiKey);
            if (apiKey == null)
            {
                Console.WriteLine($"❌ ApiKeyMiddleware - Invalid API Key for {path}");
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Invalid or expired API Key\"}");
                return;
            }

            Console.WriteLine($"✅ ApiKeyMiddleware - Valid API Key for {path}");
            context.Items["TenantId"] = apiKey.TenantId;
            context.Items["ApiKeyId"] = apiKey.Id;

            await _next(context);
        }
    }
}