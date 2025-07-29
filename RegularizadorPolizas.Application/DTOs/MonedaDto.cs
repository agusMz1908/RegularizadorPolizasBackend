using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class MonedaDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Codigo { get; set; } = string.Empty;  // DOL, PES, EU, RS, UF

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;  // Dólar Americano, Peso Uruguayo, etc.

        [StringLength(5)]
        public string? Simbolo { get; set; }  // $, €, etc.

        public bool Activa { get; set; } = true;

        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}