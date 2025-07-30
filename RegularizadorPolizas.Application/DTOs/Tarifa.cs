namespace RegularizadorPolizas.Application.DTOs
{
    public class TarifaDto
    {
        public int Id { get; set; }
        public int CompaniaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public bool Activa { get; set; } = true;
    }

    public class TarifaLookupDto
    {
        public int Id { get; set; }
        public int CompaniaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Codigo { get; set; }
    }

    public class TarifaStatsDto
    {
        public int CompaniaId { get; set; }
        public string? CompaniaNombre { get; set; }
        public int TotalTarifas { get; set; }
        public List<TarifaLookupDto> TarifasPopulares { get; set; } = new();
    }
}