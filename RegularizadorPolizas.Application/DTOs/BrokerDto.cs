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
        public int TotalPolizas { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    public class BrokerLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
    }
}