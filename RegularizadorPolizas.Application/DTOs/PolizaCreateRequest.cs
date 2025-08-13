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
        public int Seccod { get; set; }

        [Required(ErrorMessage = "El código de cliente es requerido")]
        public int Clinro { get; set; }

        public string Conpol { get; set; } = string.Empty;
        public string Confchdes { get; set; } = string.Empty;

        public string Confchhas { get; set; } = string.Empty;

        public decimal Conpremio { get; set; }

        public string Asegurado { get; set; } = string.Empty;

        #endregion

        #region CAMPOS DE CONTROL Y ESTADO
        public string? Contra { get; set; }
        public string? Congesti { get; set; }
        public string? Congeses { get; set; }
        public string? Convig { get; set; }
        public string? Consta { get; set; }

        #endregion

        #region DATOS DEL VEHÍCULO
        public string? Conmaraut { get; set; }
        public int? Conanioaut { get; set; }
        public string? Conmataut { get; set; }
        public string? Conmotor { get; set; }
        public string? Conchasis { get; set; }
        public string? Conpadaut { get; set; }

        #endregion

        #region DATOS COMERCIALES Y FINANCIEROS

        public decimal? Contot { get; set; }
        public int? Concuo { get; set; } = 1;

        public decimal? Conimp { get; set; }
        public string? Ramo { get; set; }
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

        public string? Condom { get; set; }
        public string? Clinom { get; set; }
        public int? Clinro1 { get; set; }

        #endregion

        #region COBERTURAS Y SEGUROS ESPECIALIZADOS
        public string? Tposegdsc { get; set; }
        public string? Concar { get; set; }
        public string? Conend { get; set; }
        public string? Forpagvid { get; set; }

        #endregion

        #region CAMPOS DE MONEDA

        public int? Moncod { get; set; }
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

        public string? Concesnom { get; set; }
        public string? Concestel { get; set; }
        public DateTime? Congesfi { get; set; }
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