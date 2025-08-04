using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class ApplyMappingRequestDto
    {
        public AzureDatosPolizaVelneoDto AzureData { get; set; } = new();
        public Dictionary<string, object> ManualMappings { get; set; } = new();
    }
}
