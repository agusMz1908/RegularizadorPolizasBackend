using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class ApplyMappingResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public object VelneoObject { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}
