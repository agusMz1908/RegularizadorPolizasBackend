namespace RegularizadorPolizas.Application.Configuration
{
    public class VelneoApiConfiguration
    {
        public const string SectionName = "VelneoAPI";

        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public bool EnableRetryPolicy { get; set; } = true;
        public RetryPolicyConfiguration RetryPolicy { get; set; } = new();
        public bool EnableLogging { get; set; } = true;
        public bool LogRequestResponse { get; set; } = false;
        public string ApiVersion { get; set; } = "v1";

        public Dictionary<string, string> DefaultHeaders { get; set; } = new();

        public VelneoEndpointsConfiguration Endpoints { get; set; } = new();
    }

    public class RetryPolicyConfiguration
    {
        public int MaxRetries { get; set; } = 3;
        public int BaseDelaySeconds { get; set; } = 1;
        public int MaxDelaySeconds { get; set; } = 30;
        public bool UseExponentialBackoff { get; set; } = true;
    }

    public class VelneoEndpointsConfiguration
    {
        public string Clients { get; set; } = "clients";
        public string Brokers { get; set; } = "brokers";
        public string Currencies { get; set; } = "currencies";
        public string Companies { get; set; } = "companies";
        public string Polizas { get; set; } = "polizas";
        public string Users { get; set; } = "users";
        public string Health { get; set; } = "health";
        public string System { get; set; } = "system";
    }
}