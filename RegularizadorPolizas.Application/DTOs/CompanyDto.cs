namespace RegularizadorPolizas.Application.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }

        // Campos principales de Velneo
        public string Comnom { get; set; } = string.Empty;
        public string Comrazsoc { get; set; } = string.Empty;
        public string Comruc { get; set; } = string.Empty;
        public string Comdom { get; set; } = string.Empty;
        public string Comtel { get; set; } = string.Empty;
        public string Comfax { get; set; } = string.Empty;
        public string Comsumodia { get; set; } = string.Empty;

        // Contadores
        public int Comcntcli { get; set; }
        public int Comcntcon { get; set; }

        // Valores monetarios
        public decimal Comprepes { get; set; }
        public decimal Compredol { get; set; }
        public decimal Comcomipe { get; set; }
        public decimal Comcomido { get; set; }
        public decimal Comtotcomi { get; set; }
        public decimal Comtotpre { get; set; }

        // Información adicional
        public string Comalias { get; set; } = string.Empty;
        public string Comlog { get; set; } = string.Empty;
        public bool Broker { get; set; }
        public string Cod_srvcompanias { get; set; } = string.Empty;
        public string No_utiles { get; set; } = string.Empty;
        public int Paq_dias { get; set; }

        // Propiedades de la aplicación
        public bool Activo { get; set; } = true;
        public int TotalPolizas { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;

        // Propiedades de compatibilidad con código anterior
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

        // Propiedades de compatibilidad
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
        public string No_utiles { get; set; } = string.Empty;
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