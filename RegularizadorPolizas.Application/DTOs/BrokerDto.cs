namespace RegularizadorPolizas.Application.DTOs
{
    public class BrokerDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }

        // Propiedades adicionales para información
        public int TotalPolizas { get; set; } 
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Para validaciones en el frontend
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    // DTO simplificado para listas/selects
    public class BrokerLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Email { get; set; }
    }
}