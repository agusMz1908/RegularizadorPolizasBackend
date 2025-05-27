namespace RegularizadorPolizas.Application.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Comnom { get; set; }
        public string Comrazsoc { get; set; }
        public string Comruc { get; set; }
        public string Comdom { get; set; }
        public string Comtel { get; set; }
        public string Comfax { get; set; }
        public string Comsumodia { get; set; }
        public int? Comcntcli { get; set; }
        public int? Comcntcon { get; set; }
        public decimal? Comprepes { get; set; }
        public decimal? Compredol { get; set; }
        public decimal? Comcomipe { get; set; }
        public decimal? Comcomido { get; set; }
        public decimal? Comtotcomi { get; set; }
        public decimal? Comtotpre { get; set; }
        public string Comalias { get; set; }
        public string Comlog { get; set; }
        public bool Broker { get; set; }
        public string Cod_srvcompanias { get; set; }
        public string No_utiles { get; set; }
        public int? Paq_dias { get; set; }
        public bool Activo { get; set; }
        public int TotalPolizas { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool PuedeEliminar => TotalPolizas == 0;
    }

    // DTO simplificado para listas/selects
    public class CompanyLookupDto
    {
        public int Id { get; set; }
        public string Comnom { get; set; }
        public string Comalias { get; set; }
        public string Comruc { get; set; }
    }
}