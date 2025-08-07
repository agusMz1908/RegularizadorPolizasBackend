using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Azure;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.Application.Mappers
{
    /// <summary>
    /// Mapper que convierte el resultado del escaneo de Azure Document Intelligence
    /// al formato esperado por Velneo
    /// </summary>
    public interface IAzureToVelneoMapper
    {
        Task<PolizaCreateRequest> MapAzureResultToCreateRequest(AzureProcessResponseDto scanResult);
        bool ValidateMinimumData(AzureProcessResponseDto scanResult);
    }

    public class AzureToVelneoMapper : IAzureToVelneoMapper
    {
        private readonly ILogger<AzureToVelneoMapper> _logger;
        private readonly IVelneoMaestrosService _velneoMaestrosService;

        public AzureToVelneoMapper(
            ILogger<AzureToVelneoMapper> logger,
            IVelneoMaestrosService velneoMaestrosService)
        {
            _logger = logger;
            _velneoMaestrosService = velneoMaestrosService;
        }

        /// <summary>
        /// Mapea el resultado del escaneo Azure al request de creación para Velneo
        /// </summary>
        public async Task<PolizaCreateRequest> MapAzureResultToCreateRequest(AzureProcessResponseDto scanResult)
        {
            try
            {
                _logger.LogInformation("🔄 Iniciando mapeo de Azure a Velneo para archivo: {Archivo}",
                    scanResult.Archivo);

                var datos = scanResult.DatosVelneo;

                // Crear el request base
                var request = new PolizaCreateRequest
                {
                    // ===== DATOS BÁSICOS DEL CLIENTE =====
                    Clinro = ParseInt(datos.DatosBasicos.Documento),
                    Clinom = datos.DatosBasicos.Asegurado,
                    Condom = datos.DatosBasicos.Domicilio,

                    // ===== DATOS DE LA PÓLIZA =====
                    Conpol = datos.DatosPoliza.NumeroPoliza,
                    Confchdes = datos.DatosPoliza.Desde?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    Confchhas = datos.DatosPoliza.Hasta?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"),
                    Conend = string.IsNullOrEmpty(datos.DatosPoliza.Endoso) ? "0" : datos.DatosPoliza.Endoso,

                    // ===== DATOS DEL VEHÍCULO =====
                    Conmaraut = datos.DatosVehiculo.MarcaModelo,
                    Conanioaut = ParseInt(datos.DatosVehiculo.Anio),
                    Conmataut = datos.DatosVehiculo.Matricula,
                    Conmotor = datos.DatosVehiculo.Motor,
                    Conchasis = datos.DatosVehiculo.Chasis,

                    // ===== CONDICIONES DE PAGO =====
                    Conpremio = datos.CondicionesPago.Premio,
                    Contot = datos.CondicionesPago.Total,
                    Concuo = datos.CondicionesPago.Cuotas,
                    Moncod = datos.DatosCobertura.CodigoMoneda,

                    // ===== TRAMITE Y ESTADO =====
                    Contra = MapearTramite(datos.DatosBasicos.Tramite),
                    Consta = string.IsNullOrEmpty(datos.DatosBasicos.Estado) ? "1" : datos.DatosBasicos.Estado,

                    // ===== GESTIÓN =====
                    Conges = datos.DatosBasicos.Asignado ?? "SISTEMA.IA",
                    Congesti = "1", // En gestión
                    Congeses = "4", // Pendiente
                    Convig = "2",   // Vigente

                    // ===== OBSERVACIONES =====
                    Observaciones = ConstruirObservaciones(datos.Observaciones),

                    // ===== VALORES POR DEFECTO =====
                    Conresciv = 40,
                    Conbonnsin = (int)(datos.Bonificaciones?.Descuentos ?? 0m), // Convertir decimal a int
                    Conbonant = 0,
                    Conimp = datos.Bonificaciones?.ImpuestoMSP ?? 0,

                    // ===== NOMBRE DEL ASEGURADO (CAMPO REQUERIDO) =====
                    Asegurado = datos.DatosBasicos.Asegurado,

                    // Marcar como procesado con IA
                    ProcesadoConIA = true,
                    FechaCreacion = DateTime.Now
                };

                // ===== MAPEOS QUE REQUIEREN BÚSQUEDA EN MAESTROS =====

                // Mapear compañía
                var companiaAlias = ExtraerCompaniaDePoliza(datos.DatosPoliza);
                request.Comcod = await ObtenerCompaniaId(companiaAlias);
                request.Com_alias = companiaAlias;

                // Mapear sección/ramo
                request.Seccod = await ObtenerSeccionId(datos.DatosPoliza.Ramo);
                request.Ramo = datos.DatosPoliza.Ramo;

                // Mapear corredor
                request.Corrnom = await ObtenerCorredorId(datos.DatosBasicos.Corredor);

                // Mapear categoría del vehículo
                if (!string.IsNullOrEmpty(datos.DatosVehiculo.Categoria))
                {
                    request.Catdsc = await _velneoMaestrosService.ObtenerCategoriaIdPorNombre(datos.DatosVehiculo.Categoria);
                }
                else
                {
                    request.Catdsc = 20; // Valor por defecto para AUTOMOVIL
                }

                // Mapear destino/uso del vehículo
                if (!string.IsNullOrEmpty(datos.DatosVehiculo.Destino))
                {
                    request.Desdsc = await _velneoMaestrosService.ObtenerDestinoIdPorNombre(datos.DatosVehiculo.Destino);
                }
                else if (!string.IsNullOrEmpty(datos.DatosVehiculo.Uso))
                {
                    request.Desdsc = await MapearUsoPorDestino(datos.DatosVehiculo.Uso);
                }
                else
                {
                    request.Desdsc = 1; // PARTICULAR por defecto
                }

                // Mapear calidad
                if (!string.IsNullOrEmpty(datos.DatosVehiculo.Calidad))
                {
                    request.Caldsc = await _velneoMaestrosService.ObtenerCalidadIdPorNombre(datos.DatosVehiculo.Calidad);
                }
                else
                {
                    request.Caldsc = 2; // STANDARD por defecto
                }

                // Mapear combustible
                request.Combustible = MapearCombustible(datos.DatosVehiculo.Combustible);

                // Mapear forma de pago
                request.Forpagvid = MapearFormaPago(datos.CondicionesPago.FormaPago);

                // Mapear tipo de cobertura
                request.Tposegdsc = datos.DatosCobertura.Cobertura ?? "TERCEROS COMPLETO";

                // Mapear información adicional del cliente
                request.Email = datos.DatosBasicos.Email;
                request.Telefono = datos.DatosBasicos.Telefono;
                request.Departamento = datos.DatosBasicos.Departamento ?? "MONTEVIDEO";
                request.Localidad = datos.DatosBasicos.Localidad ?? "MONTEVIDEO";

                // Agregar marca y modelo separados si es posible
                if (!string.IsNullOrEmpty(datos.DatosVehiculo.Marca))
                {
                    request.Marca = datos.DatosVehiculo.Marca;
                    request.Modelo = datos.DatosVehiculo.Modelo;
                }
                else
                {
                    // Intentar separar marca y modelo del campo combinado
                    var (marca, modelo) = SepararMarcaModelo(datos.DatosVehiculo.MarcaModelo);
                    request.Marca = marca;
                    request.Modelo = modelo;
                }

                // Certificado
                request.Certificado = ExtractCertificateNumber(datos.DatosPoliza.Certificado);

                // Nota: TipoMovimiento no existe en PolizaCreateRequest
                // Si necesitas guardarlo, podrías usar el campo Observaciones o Tramite
                if (!string.IsNullOrEmpty(datos.DatosPoliza.TipoMovimiento))
                {
                    // Mapear TipoMovimiento a Tramite si corresponde
                    request.Tramite = MapearTipoMovimientoATramite(datos.DatosPoliza.TipoMovimiento);
                }

                _logger.LogInformation("✅ Mapeo completado exitosamente para póliza: {NumeroPoliza}",
                    request.Conpol);

                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al mapear Azure result a PolizaCreateRequest");
                throw new ApplicationException("Error al mapear los datos escaneados", ex);
            }
        }

        /// <summary>
        /// Valida que el resultado del escaneo tenga los datos mínimos requeridos
        /// </summary>
        public bool ValidateMinimumData(AzureProcessResponseDto scanResult)
        {
            if (scanResult?.DatosVelneo == null)
                return false;

            var datos = scanResult.DatosVelneo;
            var errores = new List<string>();

            // Validar campos críticos
            if (string.IsNullOrWhiteSpace(datos.DatosPoliza?.NumeroPoliza))
                errores.Add("Número de póliza");

            if (string.IsNullOrWhiteSpace(datos.DatosBasicos?.Asegurado))
                errores.Add("Asegurado");

            if (datos.DatosPoliza?.Desde == null)
                errores.Add("Fecha desde");

            if (datos.DatosPoliza?.Hasta == null)
                errores.Add("Fecha hasta");

            if (datos.CondicionesPago?.Premio <= 0)
                errores.Add("Premio");

            if (errores.Any())
            {
                _logger.LogWarning("⚠️ Validación fallida. Campos faltantes: {Campos}",
                    string.Join(", ", errores));
                return false;
            }

            // Verificar el porcentaje de completitud
            if (scanResult.PorcentajeCompletitud < 70)
            {
                _logger.LogWarning("⚠️ Porcentaje de completitud bajo: {Porcentaje}%",
                    scanResult.PorcentajeCompletitud);
                return false;
            }

            return true;
        }

        // ========== MÉTODOS AUXILIARES PRIVADOS ==========

        private int ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Limpiar el valor de caracteres no numéricos comunes en documentos
            value = value.Replace(".", "").Replace("-", "").Trim();

            return int.TryParse(value, out int result) ? result : 0;
        }

        private string MapearTramite(string tramite)
        {
            if (string.IsNullOrEmpty(tramite))
                return "1";

            var tramiteUpper = tramite.ToUpperInvariant();

            return tramiteUpper switch
            {
                "NUEVO" or "NUEVA" or "EMISIÓN" or "EMISION" => "1",
                "RENOVACIÓN" or "RENOVACION" => "2",
                "ENDOSO" or "MODIFICACIÓN" or "MODIFICACION" => "3",
                "CANCELACIÓN" or "CANCELACION" or "ANULACIÓN" or "ANULACION" => "4",
                _ => "1"
            };
        }

        private string MapearTipoMovimientoATramite(string tipoMovimiento)
        {
            if (string.IsNullOrEmpty(tipoMovimiento))
                return "Nuevo";

            var tipoUpper = tipoMovimiento.ToUpperInvariant();

            // Buscar palabras clave en el tipo de movimiento
            if (tipoUpper.Contains("EMISIÓN") || tipoUpper.Contains("EMISION") || tipoUpper.Contains("NUEVA"))
                return "Nuevo";
            if (tipoUpper.Contains("RENOVACIÓN") || tipoUpper.Contains("RENOVACION"))
                return "Renovación";
            if (tipoUpper.Contains("ENDOSO") || tipoUpper.Contains("MODIFICACIÓN"))
                return "Modificación";
            if (tipoUpper.Contains("CANCELACIÓN") || tipoUpper.Contains("ANULACIÓN"))
                return "Anulación";

            return "Nuevo"; // Default
        }

        private string ExtraerCompaniaDePoliza(AzureDatosPolizaDto datosPoliza)
        {
            // Primero intentar con el campo compania
            if (!string.IsNullOrEmpty(datosPoliza.Compania))
            {
                return datosPoliza.Compania.ToUpperInvariant();
            }

            // Si no, intentar extraer del tipo de movimiento o de otros campos
            var texto = $"{datosPoliza.TipoMovimiento} {datosPoliza.NumeroPoliza}".ToUpperInvariant();

            if (texto.Contains("BSE"))
                return "BSE";
            if (texto.Contains("SURA"))
                return "SURA";
            if (texto.Contains("PORTO"))
                return "PORTO";
            if (texto.Contains("MAPFRE"))
                return "MAPFRE";
            if (texto.Contains("HDI"))
                return "HDI";

            // Por defecto BSE para el piloto
            return "BSE";
        }

        private async Task<int> ObtenerCompaniaId(string alias)
        {
            // Este mapeo debería venir de maestros, pero por ahora hardcodeamos
            var companias = new Dictionary<string, int>
            {
                { "BSE", 1 },
                { "SURA", 2 },
                { "PORTO", 3 },
                { "MAPFRE", 4 },
                { "HDI", 5 }
            };

            return companias.GetValueOrDefault(alias.ToUpperInvariant(), 1);
        }

        private async Task<int> ObtenerSeccionId(string ramo)
        {
            if (string.IsNullOrEmpty(ramo))
                return 4; // AUTOMOVILES por defecto

            var ramoUpper = ramo.ToUpperInvariant();

            var secciones = new Dictionary<string, int>
            {
                { "AUTOMOVILES", 4 },
                { "AUTOMÓVILES", 4 },
                { "AUTO", 4 },
                { "AUTOS", 4 },
                { "INCENDIO", 1 },
                { "VIDA", 2 },
                { "ACCIDENTES", 3 },
                { "HOGAR", 5 },
                { "COMERCIO", 6 }
            };

            return secciones.GetValueOrDefault(ramoUpper, 4);
        }

        private async Task<int> ObtenerCorredorId(string nombreCorredor)
        {
            if (string.IsNullOrEmpty(nombreCorredor))
                return 2; // Corredor por defecto

            // Aquí deberías buscar en la base de datos
            // Por ahora retornamos un valor por defecto
            try
            {
                var corredor = await _velneoMaestrosService.BuscarCorredorPorNombre(nombreCorredor);
                return corredor?.Id ?? 2;
            }
            catch
            {
                _logger.LogWarning("No se pudo encontrar corredor: {Nombre}. Usando default.", nombreCorredor);
                return 2;
            }
        }

        private async Task<int> MapearUsoPorDestino(string uso)
        {
            if (string.IsNullOrEmpty(uso))
                return 1;

            var usoUpper = uso.ToUpperInvariant();

            var destinos = new Dictionary<string, int>
            {
                { "PARTICULAR", 1 },
                { "PRIVADO", 1 },
                { "COMERCIAL", 2 },
                { "EMPRESA", 2 },
                { "TAXI", 3 },
                { "REMISE", 4 },
                { "UBER", 4 },
                { "ALQUILER", 5 },
                { "RENT A CAR", 5 }
            };

            return destinos.GetValueOrDefault(usoUpper, 1);
        }

        private string MapearCombustible(string combustible)
        {
            if (string.IsNullOrEmpty(combustible))
                return "NAF"; // NAFTA por defecto

            var combUpper = combustible.ToUpperInvariant();

            if (combUpper.Contains("DIESEL") || combUpper.Contains("GASOIL") || combUpper.Contains("GAS-OIL"))
                return "GAS";
            if (combUpper.Contains("NAFTA") || combUpper.Contains("GASOLINA"))
                return "NAF";
            if (combUpper.Contains("ELECTRIC") || combUpper.Contains("ELÉCTRIC"))
                return "ELE";
            if (combUpper.Contains("HÍBRID") || combUpper.Contains("HYBRID"))
                return "HIB";
            if (combUpper.Contains("GNC") || combUpper.Contains("GAS NATURAL"))
                return "GNC";
            if (combUpper.Contains("GLP") || combUpper.Contains("GAS LICUADO"))
                return "GLP";

            return "NAF";
        }

        private string MapearFormaPago(string formaPago)
        {
            if (string.IsNullOrEmpty(formaPago))
                return "EF"; // Efectivo por defecto

            var pagoUpper = formaPago.ToUpperInvariant();

            if (pagoUpper.Contains("TARJETA") && pagoUpper.Contains("CRÉDITO"))
                return "TC";
            if (pagoUpper.Contains("TARJETA") && pagoUpper.Contains("DÉBITO"))
                return "TD";
            if (pagoUpper.Contains("EFECTIVO") || pagoUpper.Contains("CONTADO"))
                return "EF";
            if (pagoUpper.Contains("TRANSFER") || pagoUpper.Contains("BANCARIA"))
                return "TB";
            if (pagoUpper.Contains("DÉBITO") && pagoUpper.Contains("AUTOMÁTICO"))
                return "DA";
            if (pagoUpper.Contains("CHEQUE"))
                return "CH";

            return "EF";
        }

        private (string marca, string modelo) SepararMarcaModelo(string marcaModelo)
        {
            if (string.IsNullOrEmpty(marcaModelo))
                return ("", "");

            // Lista de marcas conocidas para mejor separación
            var marcasConocidas = new[]
            {
                "VOLKSWAGEN", "VW", "CHEVROLET", "FORD", "FIAT", "RENAULT",
                "PEUGEOT", "CITROEN", "TOYOTA", "HONDA", "NISSAN", "SUZUKI",
                "HYUNDAI", "KIA", "MAZDA", "MITSUBISHI", "SUBARU", "BMW",
                "MERCEDES", "MERCEDES-BENZ", "AUDI", "VOLVO", "BYD", "CHERY"
            };

            var upper = marcaModelo.ToUpperInvariant();

            foreach (var marca in marcasConocidas)
            {
                if (upper.StartsWith(marca))
                {
                    var modelo = marcaModelo.Substring(marca.Length).Trim();
                    return (marca, modelo);
                }
            }

            // Si no encontramos marca conocida, tomar la primera palabra
            var partes = marcaModelo.Split(' ', 2);
            if (partes.Length == 2)
            {
                return (partes[0], partes[1]);
            }

            return (marcaModelo, "");
        }

        private string ConstruirObservaciones(AzureObservacionesDto observaciones)
        {
            if (observaciones == null)
                return "";

            var parts = new List<string>();

            if (!string.IsNullOrEmpty(observaciones.ObservacionesGenerales))
                parts.Add(observaciones.ObservacionesGenerales);

            if (!string.IsNullOrEmpty(observaciones.ObservacionesGestion))
                parts.Add($"GESTIÓN: {observaciones.ObservacionesGestion}");

            if (observaciones.NotasEscaneado?.Any() == true)
            {
                parts.Add($"ESCANEO: {string.Join("; ", observaciones.NotasEscaneado)}");
            }

            if (!string.IsNullOrEmpty(observaciones.InformacionAdicional))
                parts.Add($"INFO ADICIONAL: {observaciones.InformacionAdicional}");

            // Agregar timestamp del procesamiento
            parts.Add($"Procesado con IA: {DateTime.Now:dd/MM/yyyy HH:mm}");

            return string.Join("\n", parts);
        }

        private string ExtractCertificateNumber(string certificado)
        {
            if (string.IsNullOrEmpty(certificado))
                return "1";

            // Extraer solo números del certificado
            var numeros = System.Text.RegularExpressions.Regex.Replace(certificado, @"[^\d]", "");

            return string.IsNullOrEmpty(numeros) ? "1" : numeros;
        }
    }
}