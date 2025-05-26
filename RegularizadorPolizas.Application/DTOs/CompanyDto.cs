namespace RegularizadorPolizas.Application.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Codigo { get; set; }
        public bool Activo { get; set; }

        // Propiedades adicionales para información
        public int TotalPolizas { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Para validaciones en el frontend
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    // DTO simplificado para listas/selects
    public class CompanyLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Codigo { get; set; }
    }
}