using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class PolicyMappingResponseDto
    {
        public bool Success { get; set; }
        public PolicyMappingResultDto MappingResult { get; set; } = new();
        public DateTime Timestamp { get; set; }
        public long ProcessingTimeMs { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }
}
