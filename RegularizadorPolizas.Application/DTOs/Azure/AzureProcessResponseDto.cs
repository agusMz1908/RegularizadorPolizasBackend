namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class AzureProcessResponseDto
    {
        public string Archivo { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public long TiempoProcesamiento { get; set; }
        public string Estado { get; set; } = string.Empty;
        public AzureDatosPolizaVelneoDto DatosVelneo { get; set; } = new();
        public bool ProcesamientoExitoso => Estado == "PROCESADO_CON_SMART_EXTRACTION";
        public bool ListoParaVelneo => DatosVelneo.Metricas.TieneDatosMinimos;
        public decimal PorcentajeCompletitud => DatosVelneo.Metricas.PorcentajeCompletitud;

    }

    public class AzureDatosPolizaVelneoDto
    {
        public AzureDatosBasicosDto DatosBasicos { get; set; } = new();
        public AzureDatosPolizaDto DatosPoliza { get; set; } = new();
        public AzureDatosVehiculoDto DatosVehiculo { get; set; } = new();
        public AzureDatosCoberturaDto DatosCobertura { get; set; } = new();
        public AzureCondicionesPagoDto CondicionesPago { get; set; } = new();
        public AzureBonificacionesDto Bonificaciones { get; set; } = new();
        public AzureObservacionesDto Observaciones { get; set; } = new();

        public AzureMetricasExtraccionDto Metricas { get; set; } = new();

        public bool TieneDatosMinimos => Metricas.TieneDatosMinimos;
        public decimal PorcentajeCompletitud => Metricas.PorcentajeCompletitud;
        public int CamposCompletos => Metricas.CamposCompletos;
    }

    public class AzureDatosBasicosDto
    {
        public string Corredor { get; set; } = "";           // Extraído del escaneo
        public string Asegurado { get; set; } = "";          // Extraído del escaneo
        public string Estado { get; set; } = "";             // Lógica de negocio
        public string Domicilio { get; set; } = "";          // Extraído del escaneo
        public string Tramite { get; set; } = "";            // Extraído del escaneo
        public DateTime? Fecha { get; set; }                 // Extraído del escaneo (fecha emisión)
        public string Asignado { get; set; } = "";           // Usuario actual
        public string Tipo { get; set; } = "";               // Persona/Empresa - lógica
        public string Telefono { get; set; } = "";           // Extraído del escaneo
        public string Email { get; set; } = "";              // Extraído del escaneo
        public string Documento { get; set; } = "";          // Extraído del escaneo
        public string Departamento { get; set; } = "";       // Extraído del escaneo
        public string Localidad { get; set; } = "";          // Extraído del escaneo
        public string CodigoPostal { get; set; } = "";       // Extraído del escaneo
    }

    public class AzureDatosPolizaDto
    {
        public string Compania { get; set; } = "";           // Se elige en wizard
        public DateTime? Desde { get; set; }                 // Extraído del escaneo
        public DateTime? Hasta { get; set; }                 // Extraído del escaneo
        public string NumeroPoliza { get; set; } = "";       // Extraído del escaneo
        public string Certificado { get; set; } = "";        // Extraído del escaneo
        public string Endoso { get; set; } = "";             // Extraído del escaneo
        public string TipoMovimiento { get; set; } = "";     // Extraído del escaneo
        public string Ramo { get; set; } = "";               // Extraído del escaneo
    }

    public class AzureDatosVehiculoDto
    {
        public string MarcaModelo { get; set; } = "";        // Extraído del escaneo
        public string Marca { get; set; } = "";              // Extraído del escaneo
        public string Modelo { get; set; } = "";             // Extraído del escaneo
        public string Anio { get; set; } = "";               // Extraído del escaneo
        public string Motor { get; set; } = "";              // Extraído del escaneo
        public string Destino { get; set; } = "";            // Combo - valor extraído si existe
        public string Combustible { get; set; } = "";        // Extraído del escaneo
        public string Chasis { get; set; } = "";             // Extraído del escaneo
        public string Calidad { get; set; } = "";            // Combo - valor extraído si existe
        public string Categoria { get; set; } = "";          // Combo - valor extraído si existe
        public string Matricula { get; set; } = "";          // Extraído del escaneo
        public string Color { get; set; } = "";              // Extraído del escaneo
        public string TipoVehiculo { get; set; } = "";       // Extraído del escaneo
        public string Uso { get; set; } = "";                // Extraído del escaneo
    }

    public class AzureDatosCoberturaDto
    {
        public string Cobertura { get; set; } = "";         
        public string ZonaCirculacion { get; set; } = "";  
        public string Moneda { get; set; } = "";           
        public int CodigoMoneda { get; set; }              
    }

    public class AzureCondicionesPagoDto
    {
        public string FormaPago { get; set; } = "";        
        public decimal Premio { get; set; }          
        public decimal Total { get; set; }                 
        public decimal ValorCuota { get; set; }       
        public int Cuotas { get; set; }                     
        public string Moneda { get; set; } = "";               

        public AzureInformacionCuotasDto DetalleCuotas { get; set; } = new();
    }

    public class AzureBonificacionesDto
    {
        public List<AzureBonificacionDto> Bonificaciones { get; set; } = new();
        public decimal TotalBonificaciones { get; set; }
        public decimal Descuentos { get; set; }      
        public decimal Recargos { get; set; }               
        public decimal ImpuestoMSP { get; set; }         
    }

    public class AzureBonificacionDto
    {
        public string Tipo { get; set; } = "";
        public decimal Porcentaje { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = "";
    }

    public class AzureObservacionesDto
    {
        public string ObservacionesGenerales { get; set; } = "";
        public string ObservacionesGestion { get; set; } = "";
        public List<string> NotasEscaneado { get; set; } = new();
        public string InformacionAdicional { get; set; } = "";
    }

    public class AzureMetricasExtraccionDto
    {
        public int CamposExtraidos { get; set; }
        public int CamposCompletos { get; set; }
        public decimal PorcentajeCompletitud { get; set; }
        public bool TieneDatosMinimos { get; set; }
        public List<string> CamposFaltantes { get; set; } = new();
        public List<string> CamposConfianzaBaja { get; set; } = new();
    }

    public class AzureInformacionCuotasDto
    {
        public int CantidadTotal { get; set; }
        public List<AzureDetalleCuotaDto> Cuotas { get; set; } = new();
        public AzureDetalleCuotaDto? PrimeraCuota => Cuotas.FirstOrDefault();
        public decimal MontoPromedio => Cuotas.Any() ? Cuotas.Average(c => c.Monto) : 0;
        public bool TieneCuotasDetalladas => Cuotas.Any();
        public DateTime? PrimerVencimiento => PrimeraCuota?.FechaVencimiento;
        public decimal PrimaCuota => PrimeraCuota?.Monto ?? 0;
    }

    public class AzureDetalleCuotaDto
    {
        public int Numero { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } = "";
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
        public string Color { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty;
        public string Combustible { get; set; } = string.Empty;
        public string Uso { get; set; } = string.Empty;
        public decimal ImpuestoMSP { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public int CantidadCuotas { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public decimal Descuentos { get; set; }
        public decimal Recargos { get; set; }

        public bool TieneDatosMinimos => !string.IsNullOrEmpty(NumeroPoliza) &&
                                        !string.IsNullOrEmpty(Asegurado) &&
                                        !string.IsNullOrEmpty(Documento);

        public int CamposCompletos => GetType().GetProperties()
            .Count(p => p.PropertyType == typeof(string) && !string.IsNullOrEmpty((string?)p.GetValue(this)));

        public AzureInformacionCuotasDto DetalleCuotas { get; set; } = new();
        public DateTime? PrimerVencimiento { get; set; }
        public decimal PrimaCuota { get; set; }

        public bool TieneCuotasDetalladas => DetalleCuotas.TieneCuotasDetalladas;
        public int CuotasEncontradas => DetalleCuotas.Cuotas.Count;
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