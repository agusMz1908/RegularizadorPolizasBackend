using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.Configuration
{
    public class HybridApiConfiguration
    {
        public const string SectionName = "HybridApiConfiguration";

        public bool EnableLocalAudit { get; set; } = true;
        public bool EnableVelneoFallback { get; set; } = true;
        public bool EnableLocalCaching { get; set; } = false;
        public int CacheExpirationMinutes { get; set; } = 30;
        public bool EnableRetryPolicy { get; set; } = true;
        public int MaxRetries { get; set; } = 3;
        public int BaseDelaySeconds { get; set; } = 1;

        [Required]
        public Dictionary<string, string> EntityRouting { get; set; } = new();

        // Configuración de timeouts
        public int DefaultTimeoutSeconds { get; set; } = 30;
        public int VelneoTimeoutSeconds { get; set; } = 45;
        public int AzureTimeoutSeconds { get; set; } = 60;

        // Configuración de logging
        public bool EnableVerboseLogging { get; set; } = false;
        public bool LogRequestResponse { get; set; } = false;

        // Configuración de circuit breaker
        public bool EnableCircuitBreaker { get; set; } = true;
        public int CircuitBreakerFailureThreshold { get; set; } = 5;
        public int CircuitBreakerTimeoutSeconds { get; set; } = 30;

        public string GetRoutingDestination(string entity, string operation)
        {
            var key = $"{entity}.{operation.ToUpper()}";
            return EntityRouting.TryGetValue(key, out var destination) ? destination : "Local";
        }

        public bool ShouldRouteToVelneo(string entity, string operation)
        {
            return GetRoutingDestination(entity, operation).Equals("Velneo", StringComparison.OrdinalIgnoreCase);
        }

        public bool ShouldRouteToLocal(string entity, string operation)
        {
            return GetRoutingDestination(entity, operation).Equals("Local", StringComparison.OrdinalIgnoreCase);
        }

        public void ValidateConfiguration()
        {
            var requiredEntities = new[] { "Client", "Broker", "Currency", "Company", "Poliza", "Document" };
            var requiredOperations = new[] { "GET", "CREATE", "UPDATE", "DELETE", "SEARCH" };

            var missingRoutes = new List<string>();

            foreach (var entity in requiredEntities)
            {
                foreach (var operation in requiredOperations)
                {
                    // Excluir operaciones específicas para ciertas entidades
                    if (entity == "Document" && (operation == "UPDATE" || operation == "DELETE" || operation == "SEARCH"))
                        continue;

                    var key = $"{entity}.{operation}";
                    if (!EntityRouting.ContainsKey(key))
                    {
                        missingRoutes.Add(key);
                    }
                }
            }

            if (missingRoutes.Any())
            {
                throw new InvalidOperationException($"Missing routing configuration for: {string.Join(", ", missingRoutes)}");
            }
        }
    }

    public enum ApiDestination
    {
        Local,
        Velneo,
        Both
    }

    public class EntityRoutingRule
    {
        public string Entity { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public ApiDestination Destination { get; set; }
        public bool RequireFallback { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
    }
}