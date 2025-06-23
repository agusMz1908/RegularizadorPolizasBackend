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
                  "/api/currencies",    
                  "/api/brokers",       
                  "/api/polizas",        
                  "/error",
                  "/debug-config",
                  "/health"
            };

            if (path != null && excludedPaths.Any(excluded => path.StartsWith(excluded)))
            {
                await _next(context);
                return;
            }

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
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"API Key is missing\", \"hint\": \"Use X-Api-Key header or api_key query parameter\"}");
                return;
            }

            var apiKey = await apiKeyService.GetApiKeyAsync(extractedApiKey);
            if (apiKey == null)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Invalid or expired API Key\"}");
                return;
            }

            context.Items["TenantId"] = apiKey.TenantId;
            context.Items["ApiKeyId"] = apiKey.Id;

            await _next(context);
        }
    }
}