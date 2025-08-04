using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class MasterDataOptionsDto
    {
        public List<CategoriaDto> Categorias { get; set; } = new();
        public List<DestinoDto> Destinos { get; set; } = new();
        public List<CalidadDto> Calidades { get; set; } = new();
        public List<CombustibleDto> Combustibles { get; set; } = new();
        public List<MonedaDto> Monedas { get; set; } = new();

        // Opciones de texto plano
        public string[] EstadosPoliza { get; set; } = Array.Empty<string>();
        public string[] TiposTramite { get; set; } = Array.Empty<string>();
        public string[] EstadosBasicos { get; set; } = Array.Empty<string>();
        public string[] TiposLinea { get; set; } = Array.Empty<string>();
        public string[] FormasPago { get; set; } = Array.Empty<string>();
    }
}
