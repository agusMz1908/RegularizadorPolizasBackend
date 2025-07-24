using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaCreateRequest
    {
        [Required(ErrorMessage = "Código de compañía es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Código de compañía debe ser mayor a 0")]
        public int Comcod { get; set; }

        [Required(ErrorMessage = "Número de cliente es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Número de cliente debe ser mayor a 0")]
        public int Clinro { get; set; }

        [Required(ErrorMessage = "Número de póliza es requerido")]
        [StringLength(50, ErrorMessage = "Número de póliza no puede exceder 50 caracteres")]
        public string Conpol { get; set; } = string.Empty;

        public string Confchdes { get; set; }
        public string Confchhas { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El premio debe ser mayor o igual a 0")]
        public decimal? Conpremio { get; set; }

        [Required(ErrorMessage = "Nombre del asegurado es requerido")]
        [StringLength(200, ErrorMessage = "Nombre del asegurado no puede exceder 200 caracteres")]
        public string Asegurado { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }

        [StringLength(10, ErrorMessage = "Moneda no puede exceder 10 caracteres")]
        public string Moneda { get; set; } = "UYU";

        // Campos extendidos del vehículo
        [StringLength(100, ErrorMessage = "Descripción del vehículo no puede exceder 100 caracteres")]
        public string? Vehiculo { get; set; }

        [StringLength(50, ErrorMessage = "Marca no puede exceder 50 caracteres")]
        public string? Marca { get; set; }

        [StringLength(50, ErrorMessage = "Modelo no puede exceder 50 caracteres")]
        public string? Modelo { get; set; }

        [StringLength(50, ErrorMessage = "Motor no puede exceder 50 caracteres")]
        public string? Motor { get; set; }

        [StringLength(50, ErrorMessage = "Chasis no puede exceder 50 caracteres")]
        public string? Chasis { get; set; }

        [StringLength(20, ErrorMessage = "Matrícula no puede exceder 20 caracteres")]
        public string? Matricula { get; set; }

        [StringLength(30, ErrorMessage = "Combustible no puede exceder 30 caracteres")]
        public string? Combustible { get; set; }

        [Range(1900, 2100, ErrorMessage = "Año debe estar entre 1900 y 2100")]
        public int? Anio { get; set; }

        // Campos comerciales
        [Range(0, double.MaxValue, ErrorMessage = "La prima comercial debe ser mayor o igual a 0")]
        public decimal? PrimaComercial { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El premio total debe ser mayor o igual a 0")]
        public decimal? PremioTotal { get; set; }

        [StringLength(100, ErrorMessage = "Corredor no puede exceder 100 caracteres")]
        public string? Corredor { get; set; }

        [StringLength(100, ErrorMessage = "Plan no puede exceder 100 caracteres")]
        public string? Plan { get; set; }

        [StringLength(50, ErrorMessage = "Ramo no puede exceder 50 caracteres")]
        public string Ramo { get; set; } = "AUTOMOVILES";

        // Datos del cliente
        [StringLength(20, ErrorMessage = "Documento no puede exceder 20 caracteres")]
        public string? Documento { get; set; }

        [EmailAddress(ErrorMessage = "Email debe tener un formato válido")]
        [StringLength(100, ErrorMessage = "Email no puede exceder 100 caracteres")]
        public string? Email { get; set; }

        [StringLength(20, ErrorMessage = "Teléfono no puede exceder 20 caracteres")]
        public string? Telefono { get; set; }

        [StringLength(200, ErrorMessage = "Dirección no puede exceder 200 caracteres")]
        public string? Direccion { get; set; }

        [StringLength(100, ErrorMessage = "Localidad no puede exceder 100 caracteres")]
        public string? Localidad { get; set; }

        [StringLength(100, ErrorMessage = "Departamento no puede exceder 100 caracteres")]
        public string? Departamento { get; set; }

        // Campos técnicos
        public bool ProcesadoConIA { get; set; } = true;
    }
}