using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;
using RegularizadorPolizas.Application.DTOs;

public class DocumentExtractResult
{
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaProcesamiento { get; set; } = DateTime.Now;
    public string EstadoProcesamiento { get; set; } = string.Empty;

    // Datos específicos de la póliza (del documento)
    public DatosPolizaExtraidos DatosPoliza { get; set; } = new();

    // Datos del cliente (para búsqueda)
    public DatosClienteExtraidos DatosClienteBusqueda { get; set; } = new();

    // Metadatos
    public decimal ConfianzaExtraccion { get; set; }
    public long TiempoProcesamiento { get; set; }
    public List<string> Advertencias { get; set; } = new();
    public string RutaArchivoOriginal { get; set; } = string.Empty;
}

/// <summary>
/// Datos específicos de la póliza extraídos del documento
/// </summary>
public class DatosPolizaExtraidos
{
    // Identificación de la póliza
    public string NumeroPoliza { get; set; } = string.Empty;
    public string Endoso { get; set; } = string.Empty;
    public string TipoMovimiento { get; set; } = string.Empty;

    // Vigencia
    public DateTime? VigenciaDesde { get; set; }
    public DateTime? VigenciaHasta { get; set; }
    public DateTime? FechaEmision { get; set; }

    // Vehículo
    public string DescripcionVehiculo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? Anio { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string Padron { get; set; } = string.Empty;
    public string Motor { get; set; } = string.Empty;
    public string Chasis { get; set; } = string.Empty;
    public string Combustible { get; set; } = string.Empty;

    // Financiero
    public int PrimaComercial { get; set; } // En centavos
    public int PremioTotal { get; set; } // En centavos
    public int ImpuestoMSP { get; set; } // En centavos
    public int MontoIVA { get; set; } // En centavos
    public string Moneda { get; set; } = "UYU";
    public int CodigoMoneda { get; set; } = 1; // 1=UYU, 2=USD

    // Pago
    public string MedioPago { get; set; } = string.Empty;
    public int? CantidadCuotas { get; set; }
    public string ModoPago { get; set; } = string.Empty;

    // Corredor
    public string CorredorNombre { get; set; } = string.Empty;
    public int? CorredorNumero { get; set; }
    public string CorredorDireccion { get; set; } = string.Empty;

    // Producto/Plan
    public string Producto { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public string TipoRenovacion { get; set; } = string.Empty;
    public string Ramo { get; set; } = "AUTOMOVILES";
}

/// <summary>
/// Request para crear póliza con cliente seleccionado
/// </summary>
public class CrearPolizaConClienteRequest
{
    public int ClienteId { get; set; }
    public DatosPolizaExtraidos DatosPoliza { get; set; } = new();
    public string ArchivoOriginal { get; set; } = string.Empty;
    public string ObservacionesUsuario { get; set; } = string.Empty;
    public bool ConfirmadoPorUsuario { get; set; }
}

/// <summary>
/// Response de creación de póliza
/// </summary>
public class CrearPolizaResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public VelneoPoliza? Poliza { get; set; }
    public ClientDto? Cliente { get; set; }
    public List<string> Advertencias { get; set; } = new();
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
