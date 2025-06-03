using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Enums;
using System.Diagnostics;

namespace RegularizadorPolizas.API.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = context.TraceIdentifier;

            try
            {
                await _next(context);
                stopwatch.Stop();

                await AuditImportantRequests(context, stopwatch.ElapsedMilliseconds, true);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await AuditImportantRequests(context, stopwatch.ElapsedMilliseconds, false, ex);

                throw;
            }
        }

        private async Task AuditImportantRequests(HttpContext context, long durationMs, bool success, Exception? exception = null)
        {
            try
            {
                var path = context.Request.Path.Value?.ToLower() ?? "";
                var method = context.Request.Method.ToUpper();

                if (path.Contains("/api/audit"))
                    return;
                if (path.Contains("/health") || path.Contains("/swagger") || path.Contains("/debug-config"))
                    return;

                var auditService = context.RequestServices.GetService<IAuditService>();
                if (auditService == null)
                    return;

                var shouldAudit = ShouldAuditRequest(path, method, context.Response.StatusCode, success);

                if (!shouldAudit)
                    return;

                var eventType = DetermineEventType(path, method, success);
                var description = $"{method} {path} - {(success ? "Success" : "Error")} ({context.Response.StatusCode})";

                var additionalData = new
                {
                    Method = method,
                    Path = path,
                    StatusCode = context.Response.StatusCode,
                    DurationMs = durationMs,
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
                    RequestId = context.TraceIdentifier,
                    ContentType = context.Request.ContentType,
                    QueryString = context.Request.QueryString.Value
                };

                if (exception != null)
                {
                    await auditService.LogErrorAsync(exception, description, additionalData);
                }
                else
                {
                    await auditService.LogAsync(eventType, description, additionalData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in audit middleware");
            }
        }

        private bool ShouldAuditRequest(string path, string method, int statusCode, bool success)
        {
            if (!success || statusCode >= 400)
                return true;

            var importantPaths = new[]
            {
                "/api/auth/login",
                "/api/clients",
                "/api/polizas",
                "/api/brokers",
                "/api/companies",
                "/api/currencies",
                "/api/renovations",
                "/api/documents"
            };

            var auditableMethods = new[] { "POST", "PUT", "DELETE" };

            return importantPaths.Any(p => path.StartsWith(p)) &&
                   (auditableMethods.Contains(method) || path.Contains("/login"));
        }

        private AuditEventType DetermineEventType(string path, string method, bool success)
        {
            if (!success)
                return AuditEventType.SystemError;

            if (path.Contains("/login"))
                return AuditEventType.Login;

            return (path.ToLower(), method.ToUpper()) switch
            {
                (var p, "POST") when p.Contains("/clients") => AuditEventType.ClientCreated,
                (var p, "PUT") when p.Contains("/clients") => AuditEventType.ClientUpdated,
                (var p, "DELETE") when p.Contains("/clients") => AuditEventType.ClientDeleted,

                (var p, "POST") when p.Contains("/polizas") => AuditEventType.PolicyCreated,
                (var p, "PUT") when p.Contains("/polizas") => AuditEventType.PolicyUpdated,
                (var p, "DELETE") when p.Contains("/polizas") => AuditEventType.PolicyDeleted,

                (var p, "POST") when p.Contains("/brokers") => AuditEventType.BrokerCreated,
                (var p, "PUT") when p.Contains("/brokers") => AuditEventType.BrokerUpdated,
                (var p, "DELETE") when p.Contains("/brokers") => AuditEventType.BrokerDeleted,

                (var p, "POST") when p.Contains("/companies") => AuditEventType.CompanyCreated,
                (var p, "PUT") when p.Contains("/companies") => AuditEventType.CompanyUpdated,
                (var p, "DELETE") when p.Contains("/companies") => AuditEventType.CompanyDeleted,

                (var p, "POST") when p.Contains("/currencies") => AuditEventType.CurrencyCreated,
                (var p, "PUT") when p.Contains("/currencies") => AuditEventType.CurrencyUpdated,
                (var p, "DELETE") when p.Contains("/currencies") => AuditEventType.CurrencyDeleted,

                (var p, "POST") when p.Contains("/renovations") => AuditEventType.RenovationCreated,

                (var p, "POST") when p.Contains("/documents") => AuditEventType.DocumentUploaded,

                _ => AuditEventType.SystemInfo
            };
        }
    }

    public static class AuditMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditMiddleware>();
        }
    }
}