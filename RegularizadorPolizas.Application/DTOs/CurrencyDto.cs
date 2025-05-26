namespace RegularizadorPolizas.Application.DTOs
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Simbolo { get; set; }
        public bool Activo { get; set; }

        // Propiedades adicionales para información
        public int TotalPolizas { get; set; } // Cantidad de pólizas asociadas
        public bool EsMonedaPorDefecto { get; set; } // Si es UYU
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Para validaciones en el frontend
        public bool PuedeEliminar => TotalPolizas == 0 && !EsMonedaPorDefecto;
    }

    // DTO simplificado para listas/selects
    public class CurrencyLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Simbolo { get; set; }
    }
}