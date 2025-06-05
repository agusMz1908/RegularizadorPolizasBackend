namespace RegularizadorPolizas.Application.DTOs
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Simbolo { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public int TotalPolizas { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
        public string Codigo => Moneda;
    }

    public class CurrencyLookupDto
    {
        public int Id { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Simbolo { get; set; } = string.Empty;
        public string Codigo => Moneda;
    }

    public class CurrencyCreateDto
    {
        public string Moneda { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Simbolo { get; set; } = string.Empty;
    }

    public class CurrencySummaryDto
    {
        public int Id { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Simbolo { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int TotalPolizas { get; set; }
        public string Codigo => Moneda;
    }

    public class VelneoCurrencyResponse
    {
        public int Count { get; set; }
        public int Total_count { get; set; }
        public List<VelneoCurrencyDto> Monedas { get; set; } = new();
    }

    public class VelneoCurrencyDto
    {
        public int Id { get; set; }
        public string Moneda { get; set; } = string.Empty;
    }
}