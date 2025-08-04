using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class PolicyMappingResultDto
    {
        public Dictionary<string, MappedField> CamposMapeados { get; set; } = new();
        public List<string> CamposQueFallaronMapeo { get; set; } = new();
        public int PorcentajeExito { get; set; }
        public int CamposConAltaConfianza { get; set; }
        public int CamposConMediaConfianza { get; set; }
        public int CamposConBajaConfianza { get; set; }
        public MasterDataOptions OpcionesDisponibles { get; set; } = new();
    }

    public class MappedField
    {
        public string ValorExtraido { get; set; } = "";
        public object? ValorMapeado { get; set; }
        public int Confianza { get; set; }
        public object OpcionesDisponibles { get; set; } = new();

        public string NivelConfianza => Confianza switch
        {
            >= 90 => "high",
            >= 70 => "medium",
            >= 50 => "low",
            _ => "failed"
        };

        public bool RequiereRevision => Confianza < 70;
        public bool MapeoExitoso => ValorMapeado != null && Confianza >= 50;
    }

    public class MasterDataOptions
    {
        public List<CategoriaDto> Categorias { get; set; } = new();
        public List<DestinoDto> Destinos { get; set; } = new();
        public List<CalidadDto> Calidades { get; set; } = new();
        public List<CombustibleDto> Combustibles { get; set; } = new();
        public List<MonedaDto> Monedas { get; set; } = new();
    }
}
