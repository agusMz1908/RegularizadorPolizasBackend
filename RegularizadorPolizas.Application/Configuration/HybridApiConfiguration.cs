using System.Collections.Generic;

namespace RegularizadorPolizas.Application.Configuration
{
    public class HybridApiConfiguration
    {
        public Dictionary<string, ApiTarget> EntityRouting { get; set; } = new();
        public bool EnableLocalAudit { get; set; } = true;
        public bool EnableVelneoFallback { get; set; } = true;
        public bool EnableLocalCaching { get; set; } = false;
    }

    public enum ApiTarget
    {
        Local,
        Velneo,
        Both,
        LocalFirst,
        VelneoFirst
    }
}