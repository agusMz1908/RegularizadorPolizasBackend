using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class DepartamentoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Codigo { get; set; }
        public decimal? BonificacionInterior { get; set; }
        public string? CodigoSC { get; set; }
        public bool Activo { get; set; } = true;
    }
}