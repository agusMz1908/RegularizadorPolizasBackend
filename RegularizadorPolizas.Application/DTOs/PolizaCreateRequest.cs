using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.Application.DTOs
{
    public class PolizaCreateRequest
    {
        #region CAMPOS BÁSICOS REQUERIDOS

        [Required(ErrorMessage = "El código de compañía es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El código de compañía debe ser mayor a 0")]
        public int Comcod { get; set; }

        [Required(ErrorMessage = "El código de sección es requerido")]
        [Range(0, 9, ErrorMessage = "El código de sección debe estar entre 0-9")]
        public int Seccod { get; set; }

        [Required(ErrorMessage = "El código de cliente es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El código de cliente debe ser mayor a 0")]
        public int Clinro { get; set; }

        [Required(ErrorMessage = "El número de póliza es requerido")]
        [StringLength(256, ErrorMessage = "El número de póliza no puede exceder 256 caracteres")]
        public string Conpol { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public string Confchdes { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public string Confchhas { get; set; } = string.Empty;

        [Required(ErrorMessage = "El premio es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El premio debe ser mayor o igual a 0")]
        public decimal Conpremio { get; set; }

        [Required(ErrorMessage = "El nombre del asegurado es requerido")]
        [StringLength(128, ErrorMessage = "El nombre del asegurado no puede exceder 128 caracteres")]
        public string Asegurado { get; set; } = string.Empty;

        #endregion

        #region CAMPOS DE CONTROL Y ESTADO

        [StringLength(256, ErrorMessage = "El trámite no puede exceder 256 caracteres")]
        public string? Contra { get; set; }

        [StringLength(256, ErrorMessage = "El estado de gestión no puede exceder 256 caracteres")]
        public string? Congesti { get; set; }

        [StringLength(256, ErrorMessage = "El estado de gestión no puede exceder 256 caracteres")]
        public string? Congeses { get; set; }

        [StringLength(256, ErrorMessage = "El estado de la póliza no puede exceder 256 caracteres")]
        public string? Convig { get; set; }

        [StringLength(256, ErrorMessage = "La forma de pago no puede exceder 256 caracteres")]
        public string? Consta { get; set; }

        #endregion

        #region DATOS DEL VEHÍCULO

        [StringLength(128, ErrorMessage = "La marca no puede exceder 128 caracteres")]
        public string? Conmaraut { get; set; }

        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100")]
        public int? Conanioaut { get; set; }

        [StringLength(64, ErrorMessage = "La matrícula no puede exceder 64 caracteres")]
        public string? Conmataut { get; set; }

        [StringLength(40, ErrorMessage = "El motor no puede exceder 40 caracteres")]
        public string? Conmotor { get; set; }

        [StringLength(40, ErrorMessage = "El chasis no puede exceder 40 caracteres")]
        public string? Conchasis { get; set; }

        [StringLength(40, ErrorMessage = "El padrón no puede exceder 40 caracteres")]
        public string? Conpadaut { get; set; }

        #endregion

        #region DATOS COMERCIALES Y FINANCIEROS

        public decimal? Contot { get; set; }

        [Range(1, 12, ErrorMessage = "Las cuotas deben estar entre 1 y 12")]
        public int? Concuo { get; set; } = 1;

        public decimal? Conimp { get; set; }

        [StringLength(128, ErrorMessage = "El ramo no puede exceder 128 caracteres")]
        public string? Ramo { get; set; } = "AUTOMOVILES";

        [StringLength(40, ErrorMessage = "El alias de compañía no puede exceder 40 caracteres")]
        public string? Com_alias { get; set; }

        #endregion

        #region CLASIFICACIONES (IDs de maestros)

        public int? Catdsc { get; set; }
        public int? Desdsc { get; set; }
        public int? Caldsc { get; set; }
        public int? Flocod { get; set; }
        public int? Tarcod { get; set; }
        public int? Corrnom { get; set; }

        #endregion

        #region DATOS DEL CLIENTE/ASEGURADO

        [StringLength(256, ErrorMessage = "La dirección no puede exceder 256 caracteres")]
        public string? Condom { get; set; }
        [StringLength(128, ErrorMessage = "El nombre del cliente no puede exceder 128 caracteres")]
        public string? Clinom { get; set; }
        public int? Clinro1 { get; set; }

        #endregion

        #region COBERTURAS Y SEGUROS ESPECIALIZADOS

        [StringLength(128, ErrorMessage = "La cobertura no puede exceder 128 caracteres")]
        public string? Tposegdsc { get; set; }
        [StringLength(256, ErrorMessage = "El certificado no puede exceder 256 caracteres")]
        public string? Concar { get; set; }
        [StringLength(40, ErrorMessage = "El endoso no puede exceder 40 caracteres")]
        public string? Conend { get; set; }
        [StringLength(256, ErrorMessage = "La forma de pago de vida no puede exceder 256 caracteres")]
        public string? Forpagvid { get; set; }

        #endregion

        #region CAMPOS DE MONEDA

        [Range(1, int.MaxValue, ErrorMessage = "El código de moneda debe ser mayor a 0")]
        public int? Moncod { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El código de moneda de condiciones de pago debe ser mayor a 0")]
        public int? Conviamon { get; set; }

        #endregion

        #region CAMPOS ADICIONALES DE VEHÍCULOS

        public int? Conclaaut { get; set; }
        public int? Condedaut { get; set; }
        public int? Conresciv { get; set; }
        public int? Conbonnsin { get; set; }
        public int? Conbonant { get; set; }
        public int? Concaraut { get; set; }
        public int? Concapaut { get; set; }

        #endregion

        #region CAMPOS DE GESTIÓN Y PROCESO

        [StringLength(64, ErrorMessage = "El nombre del cesionario no puede exceder 64 caracteres")]
        public string? Concesnom { get; set; }

        [StringLength(64, ErrorMessage = "El teléfono del cesionario no puede exceder 64 caracteres")]
        public string? Concestel { get; set; }

        public DateTime? Congesfi { get; set; }

        [StringLength(128, ErrorMessage = "La gestión no puede exceder 128 caracteres")]
        public string? Conges { get; set; }

        #endregion

        #region CAMPOS DE AUDITORÍA

        public string? Observaciones { get; set; }
        public bool ProcesadoConIA { get; set; } = false;
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        #endregion

        #region CAMPOS LEGACY (mantener compatibilidad)
        public string? Vehiculo { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Motor { get; set; }
        public string? Chasis { get; set; }
        public string? Matricula { get; set; }
        public string? Combustible { get; set; }
        public int? Anio { get; set; }
        public decimal? PrimaComercial { get; set; }
        public decimal? PremioTotal { get; set; }
        public string? Corredor { get; set; }
        public string? Plan { get; set; }
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Localidad { get; set; }
        public string? Departamento { get; set; }
        public string? Moneda { get; set; }

        public int? SeccionId { get; set; }
        public string? Estado { get; set; }
        public string? Tramite { get; set; }
        public string? EstadoPoliza { get; set; }
        public int? CalidadId { get; set; }
        public int? DestinoId { get; set; }
        public int? CategoriaId { get; set; }
        public string? TipoVehiculo { get; set; }
        public string? Uso { get; set; }
        public string? FormaPago { get; set; }
        public int? CantidadCuotas { get; set; }
        public decimal? ValorCuota { get; set; }
        public string? Tipo { get; set; }
        public string? Cobertura { get; set; }
        public string? Certificado { get; set; }
        public string? Calidad { get; set; }
        public string? Categoria { get; set; }
        public string? Destino { get; set; }

        #endregion
    }
}