using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class CoberturaDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string? Codigo { get; set; }
        public bool Activo { get; set; } = true;
        public string? Observaciones { get; set; }
    }
}