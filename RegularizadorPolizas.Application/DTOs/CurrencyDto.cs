namespace RegularizadorPolizas.Application.DTOs
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Simbolo { get; set; }
        public bool Activo { get; set; }
        public int TotalPolizas { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    public class CurrencyLookupDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Simbolo { get; set; }
    }
}