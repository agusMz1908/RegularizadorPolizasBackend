using RegularizadorPolizas.Application.Converters;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Comnom { get; set; } = string.Empty;
        public string Comrazsoc { get; set; } = string.Empty;
        public string Comruc { get; set; } = string.Empty;
        public string Comdom { get; set; } = string.Empty;
        public string Comtel { get; set; } = string.Empty;
        public string Comfax { get; set; } = string.Empty;
        public string Comsumodia { get; set; } = string.Empty;
        public int Comcntcli { get; set; }
        public int Comcntcon { get; set; }
        public decimal Comprepes { get; set; }
        public decimal Compredol { get; set; }
        public decimal Comcomipe { get; set; }
        public decimal Comcomido { get; set; }
        public decimal Comtotcomi { get; set; }
        public decimal Comtotpre { get; set; }
        public string Comalias { get; set; } = string.Empty;
        public string Comlog { get; set; } = string.Empty;
        public bool Broker { get; set; }
        public string Cod_srvcompanias { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int No_utiles { get; set; }
        public int Paq_dias { get; set; }
        public bool Activo { get; set; } = true;
        public int TotalPolizas { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
        public string Nombre => Comnom;
        public string Alias => Comalias;
        public string Codigo => Cod_srvcompanias;
    }

    public class CompanyLookupDto
    {
        public int Id { get; set; }
        public string Comnom { get; set; } = string.Empty;
        public string Comalias { get; set; } = string.Empty;
        public string Cod_srvcompanias { get; set; } = string.Empty;
        public string Nombre => Comnom;
        public string Alias => Comalias;
        public string Codigo => Cod_srvcompanias;
    }

    public class CompanyCreateDto
    {
        public string Comnom { get; set; } = string.Empty;
        public string Comrazsoc { get; set; } = string.Empty;
        public string Comruc { get; set; } = string.Empty;
        public string Comdom { get; set; } = string.Empty;
        public string Comtel { get; set; } = string.Empty;
        public string Comfax { get; set; } = string.Empty;
        public string Comalias { get; set; } = string.Empty;
        public bool Broker { get; set; }
        public string Cod_srvcompanias { get; set; } = string.Empty;
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int No_utiles { get; set; }
        public int Paq_dias { get; set; }
    }

    public class CompanySummaryDto
    {
        public int Id { get; set; }
        public string Comnom { get; set; } = string.Empty;
        public string Comalias { get; set; } = string.Empty;
        public string Cod_srvcompanias { get; set; } = string.Empty;
        public bool Broker { get; set; }
        public bool Activo { get; set; }
        public int TotalPolizas { get; set; }
    }
}