using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegularizadorPolizas.Domain.Entities
{
    public class ProcessDocument
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string NombreArchivo { get; set; }
        [Required]
        [StringLength(255)]
        public string RutaArchivo { get; set; }
        [StringLength(50)]
        public string TipoDocumento { get; set; }
        [StringLength(50)]
        public string EstadoProcesamiento { get; set; } = "PENDIENTE";
        public string ResultadoJson { get; set; }
        [Column(TypeName = "decimal(8,2)")]
        public decimal? TiempoProcessamiento { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal? CostoProcessamiento { get; set; }
        public int? NumeroPaginas { get; set; }
        [StringLength(20)]
        public string CodigoCompania { get; set; }
        public int? CompaniaId { get; set; }
        [StringLength(1000)]
        public string MensajeError { get; set; }
        [Column(TypeName = "decimal(5,4)")]
        public decimal? NivelConfianza { get; set; }
        public DateTime? FechaInicioProcesamiento { get; set; }
        public DateTime? FechaFinProcesamiento { get; set; }
        public bool? EnviadoVelneo { get; set; }
        public DateTime? FechaEnvioVelneo { get; set; }
        [StringLength(500)]
        public string RespuestaVelneo { get; set; }
        public long? TamanoArchivo { get; set; }
        [StringLength(100)]
        public string TipoMime { get; set; }
        [StringLength(32)]
        public string HashArchivo { get; set; }
        public string MetadatosJson { get; set; }
        public int Prioridad { get; set; } = 2;
        public int IntentosProcesamiento { get; set; } = 0;
        public int MaxIntentos { get; set; } = 3;
        public int? PolizaId { get; set; }
        public DateTime? FechaProcesamiento { get; set; }
        public int? UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [ForeignKey("PolizaId")]
        public virtual Poliza Poliza { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual User User { get; set; }

        [ForeignKey("CompaniaId")]
        public virtual Company Company { get; set; }

        [NotMapped]
        public string EstadoActual
        {
            get
            {
                if (!string.IsNullOrEmpty(MensajeError))
                    return "ERROR";

                if (EnviadoVelneo == true)
                    return "ENVIADO_VELNEO";

                if (EstadoProcesamiento == "COMPLETADO")
                    return "PROCESADO";

                if (FechaInicioProcesamiento.HasValue && !FechaFinProcesamiento.HasValue)
                    return "PROCESANDO";

                return EstadoProcesamiento ?? "PENDIENTE";
            }
        }

        [NotMapped]
        public TimeSpan? DuracionProcesamiento
        {
            get
            {
                if (FechaInicioProcesamiento.HasValue && FechaFinProcesamiento.HasValue)
                    return FechaFinProcesamiento - FechaInicioProcesamiento;
                return null;
            }
        }


        [NotMapped]
        public bool EsExitoso =>
            EstadoProcesamiento == "COMPLETADO" &&
            string.IsNullOrEmpty(MensajeError) &&
            (EnviadoVelneo == true || EnviadoVelneo == null);


        [NotMapped]
        public decimal? CostoPorPagina
        {
            get
            {
                if (CostoProcessamiento.HasValue && NumeroPaginas.HasValue && NumeroPaginas > 0)
                    return CostoProcessamiento / NumeroPaginas;
                return null;
            }
        }
    }
}
public static class EstadosProcessDocument
{
    public const string PENDIENTE = "PENDIENTE";
    public const string PROCESANDO = "PROCESANDO";
    public const string COMPLETADO = "COMPLETADO";
    public const string ERROR = "ERROR";
    public const string ENVIADO_VELNEO = "ENVIADO_VELNEO";
    public const string ERROR_VELNEO = "ERROR_VELNEO";
}