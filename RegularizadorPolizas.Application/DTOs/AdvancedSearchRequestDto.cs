using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class AdvancedSearchRequestDto
    {
        [StringLength(255)]
        public string? Nombre { get; set; }

        [StringLength(50)]
        public string? Documento { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        [StringLength(500)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Localidad { get; set; }

        [StringLength(100)]
        public string? Departamento { get; set; }

        [Range(1, 100)]
        public int Limite { get; set; } = 20;

        public List<string> GetNonEmptyCriteria()
        {
            var criterios = new List<string>();

            if (!string.IsNullOrWhiteSpace(Nombre))
                criterios.Add($"Nombre: {Nombre}");

            if (!string.IsNullOrWhiteSpace(Documento))
                criterios.Add($"Documento: {Documento}");

            if (!string.IsNullOrWhiteSpace(Email))
                criterios.Add($"Email: {Email}");

            if (!string.IsNullOrWhiteSpace(Telefono))
                criterios.Add($"Teléfono: {Telefono}");

            if (!string.IsNullOrWhiteSpace(Direccion))
                criterios.Add($"Dirección: {Direccion}");

            if (!string.IsNullOrWhiteSpace(Localidad))
                criterios.Add($"Localidad: {Localidad}");

            if (!string.IsNullOrWhiteSpace(Departamento))
                criterios.Add($"Departamento: {Departamento}");

            return criterios;
        }
        public bool HasAnyCriteria()
        {
            return !string.IsNullOrWhiteSpace(Nombre) ||
                   !string.IsNullOrWhiteSpace(Documento) ||
                   !string.IsNullOrWhiteSpace(Email) ||
                   !string.IsNullOrWhiteSpace(Telefono) ||
                   !string.IsNullOrWhiteSpace(Direccion) ||
                   !string.IsNullOrWhiteSpace(Localidad) ||
                   !string.IsNullOrWhiteSpace(Departamento);
        }
    }
}