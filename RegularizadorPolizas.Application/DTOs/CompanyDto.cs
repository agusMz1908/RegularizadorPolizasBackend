namespace RegularizadorPolizas.Application.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Codigo { get; set; }
        public bool Activo { get; set; }
        public int TotalPolizas { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    public class CompanyLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
    }
}