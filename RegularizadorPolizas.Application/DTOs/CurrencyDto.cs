namespace RegularizadorPolizas.Application.DTOs
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Moneda { get; set; }
        public bool Activo { get; set; }
        public int TotalPolizas { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    // DTO simplificado para listas/selects
    public class CurrencyLookupDto
    {
        public int Id { get; set; }
        public string Moneda { get; set; }
    }
}