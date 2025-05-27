namespace RegularizadorPolizas.Application.DTOs
{
    public class BrokerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Observaciones { get; set; }
        public string Foto { get; set; } 
        public bool Activo { get; set; }

        public int TotalPolizas { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    // DTO simplificado para listas/selects
    public class BrokerLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Telefono { get; set; }
    }
}