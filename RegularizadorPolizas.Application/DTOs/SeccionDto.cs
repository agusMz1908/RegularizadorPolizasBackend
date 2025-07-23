using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class SeccionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la sección es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Seccion { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El icono no puede exceder 50 caracteres")]
        public string Icono { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }
    }
    public class SeccionLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }

    public class CreateSeccionDto
    {
        [Required(ErrorMessage = "El nombre de la sección es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El icono no puede exceder 50 caracteres")]
        public string Icono { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }

    public class UpdateSeccionDto
    {
        [Required(ErrorMessage = "El nombre de la sección es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El icono no puede exceder 50 caracteres")]
        public string Icono { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}