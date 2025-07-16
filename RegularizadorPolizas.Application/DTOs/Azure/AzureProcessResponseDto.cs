namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class AzureProcessResponseDto
    {
        public string Archivo { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public long TiempoProcesamiento { get; set; }
        public string Estado { get; set; } = string.Empty;
        public AzureDatosFormateadosDto DatosFormateados { get; set; } = new();
        public AzureBusquedaClienteDto BusquedaCliente { get; set; } = new();
        public string SiguientePaso { get; set; } = string.Empty;
        public AzureResumenDto Resumen { get; set; } = new();
        public bool ProcesamientoExitoso => Estado == "PROCESADO_CON_SMART_EXTRACTION";
        public bool RequiereIntervencion => BusquedaCliente.RequiereIntervencion;
        public bool ListoParaVelneo => Resumen.ListoParaVelneo;
    }
    public class AzureDatosFormateadosDto
    {
        public string NumeroPoliza { get; set; } = string.Empty;
        public string Asegurado { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Motor { get; set; } = string.Empty;
        public string Chasis { get; set; } = string.Empty;
        public decimal PrimaComercial { get; set; }
        public decimal PremioTotal { get; set; }
        public DateTime? VigenciaDesde { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public string Corredor { get; set; } = string.Empty;
        public string Plan { get; set; } = string.Empty;
        public string Ramo { get; set; } = string.Empty;
        public string Anio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public bool TieneDatosMinimos => !string.IsNullOrEmpty(NumeroPoliza) &&
                                        !string.IsNullOrEmpty(Asegurado) &&
                                        !string.IsNullOrEmpty(Documento);

        public int CamposCompletos => GetType().GetProperties()
            .Count(p => p.PropertyType == typeof(string) && !string.IsNullOrEmpty((string?)p.GetValue(this)));
    }

    public class AzureBusquedaClienteDto
    {
        public string TipoResultado { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool RequiereIntervencion { get; set; } = true;
        public int ClientesEncontrados { get; set; }
        public List<AzureClienteMatchDto> Matches { get; set; } = new();
        public bool TieneMatches => ClientesEncontrados > 0;
        public bool EsMatchExacto => TipoResultado == "MatchExacto";
        public bool SonMultiplesMatches => TipoResultado == "MultiplesMatches";
        public AzureClienteMatchDto? MejorMatch => Matches.OrderByDescending(m => m.Score).FirstOrDefault();
    }

    public class AzureClienteMatchDto
    {
        public AzureClienteInfoDto Cliente { get; set; } = new();
        public float Score { get; set; }
        public string Criterio { get; set; } = string.Empty;
        public List<string> Coincidencias { get; set; } = new();
        public bool EsAltaConfianza => Score >= 95;
        public bool EsMediaConfianza => Score >= 70 && Score < 95;
        public bool EsBajaConfianza => Score < 70;
        public string NivelConfianza => EsAltaConfianza ? "Alto" : EsMediaConfianza ? "Medio" : "Bajo";
    }
    public class AzureClienteInfoDto
    {
        public int? Id { get; set; }  
        public string Nombre { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public bool TieneContacto => !string.IsNullOrEmpty(Telefono) || !string.IsNullOrEmpty(Email);
        public string ContactoPrincipal => !string.IsNullOrEmpty(Email) ? Email : Telefono ?? "Sin contacto";
    }

    public class AzureResumenDto
    {
        public bool ProcesamientoExitoso { get; set; }
        public string NumeroPolizaExtraido { get; set; } = string.Empty;
        public string ClienteExtraido { get; set; } = string.Empty;
        public string DocumentoExtraido { get; set; } = string.Empty;
        public string VehiculoExtraido { get; set; } = string.Empty;
        public bool ClienteEncontrado { get; set; }
        public bool ListoParaVelneo { get; set; }
        public string EstadoGeneral => ListoParaVelneo ? "Listo para Velneo" :
                                      ClienteEncontrado ? "Cliente encontrado" :
                                      ProcesamientoExitoso ? "Procesado" : "Error";

        public int PorcentajeCompletitud
        {
            get
            {
                var campos = new[] { NumeroPolizaExtraido, ClienteExtraido, DocumentoExtraido, VehiculoExtraido };
                var camposCompletos = campos.Count(c => !string.IsNullOrEmpty(c));
                return (int)((camposCompletos / (float)campos.Length) * 100);
            }
        }
    }
}