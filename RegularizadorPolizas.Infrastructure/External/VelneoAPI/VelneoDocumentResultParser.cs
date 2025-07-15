using System.Globalization;
using System.Text.RegularExpressions;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace RegularizadorPolizas.Infrastructure.External
{
    public class VelneoDocumentResultParser
    {
        private readonly ILogger<VelneoDocumentResultParser>? _logger;

        public VelneoDocumentResultParser(ILogger<VelneoDocumentResultParser> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Parsea el resultado de Azure Document Intelligence a un objeto VelneoPoliza
        /// </summary>
        public VelneoPoliza ParseToVelneoPoliza(JObject azureResult)
        {
            var poliza = new VelneoPoliza
            {
                // Campos obligatorios de Velneo con valores por defecto
                Comcod = 1,           // Código de compañía por defecto
                Seccod = 4,           // Sección AUTO
                Moncod = 1,           // Peso uruguayo por defecto
                Convig = "1",         // Activa
                Consta = "1",         // Estado activo
                Contra = "2",         // Tipo de contrato
                Ramo = "AUTOMOVILES", // Ramo vehículos

                // Metadatos
                Last_update = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Ingresado = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Enviado = false,
                Leer = false
            };

            try
            {
                var analyzeResult = azureResult["analyzeResult"];
                if (analyzeResult == null)
                {
                    _logger?.LogWarning("analyzeResult is null in Azure response");
                    return poliza;
                }

                // Estrategia 1: Extraer desde documents (modelo custom de Azure)
                var documents = analyzeResult["documents"] as JArray;
                if (documents?.Count > 0)
                {
                    _logger?.LogDebug("Extracting from documents array with {Count} documents", documents.Count);
                    ExtractFromDocuments(poliza, documents[0]);
                }

                // Estrategia 2: Extraer desde content si es necesario (fallback)
                var content = analyzeResult["content"]?.ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    _logger?.LogDebug("Applying content fallback extraction");
                    ExtractFromContentFallback(poliza, content);
                }

                // Estrategia 3: Validaciones y completado de campos
                CompletarCamposCalculados(poliza);
                ValidarYCorregirDatos(poliza);

                _logger?.LogInformation("Successfully parsed document to VelneoPoliza - Policy: {PolicyNumber}, Client: {ClientName}, Vehicle: {Vehicle}",
                    poliza.Conpol, poliza.Clinom, poliza.Conmaraut);

                return poliza;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error parsing Azure Document Intelligence result for Velneo");
                poliza.Observaciones = $"Error en parsing: {ex.Message} - {DateTime.Now:yyyy-MM-dd HH:mm}";
                return poliza;
            }
        }

        #region Extracción desde Documents (Modelo Custom Azure)

        private void ExtractFromDocuments(VelneoPoliza poliza, JToken document)
        {
            var fields = document["fields"];
            if (fields == null) return;

            // === DATOS DE LA PÓLIZA ===
            ExtractFieldValue(fields, "poliza.numero", value => {
                poliza.Conpol = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "poliza.endoso", value => {
                poliza.Conend = LimpiarTexto(value);
            });

            // === DATOS DEL ASEGURADO ===
            ExtractFieldValue(fields, "asegurado.nombre", value => {
                poliza.Clinom = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "asegurado.documento", value => {
                poliza.Cliruc = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "asegurado.direccion", value => {
                poliza.Condom = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "asegurado.email", value => {
                poliza.Cliemail = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "asegurado.telefono", value => {
                poliza.Clitelcel = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "asegurado.localidad", value => {
                poliza.Clilocnom = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "asegurado.departamento", value => {
                poliza.Clidptnom = LimpiarTexto(value);
            });

            // === DATOS DEL VEHÍCULO ===
            ExtractFieldValue(fields, "vehiculo.marca", value => {
                var vehicleInfo = LimpiarTexto(value);
                // Si ya tenemos descripción completa, mantenerla; sino, usar marca
                if (string.IsNullOrEmpty(poliza.Conmaraut))
                {
                    poliza.Conmaraut = vehicleInfo;
                }
            });

            ExtractFieldValue(fields, "vehiculo.descripcion", value => {
                poliza.Conmaraut = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "vehiculo.modelo", value => {
                var modelo = LimpiarTexto(value);
                // Combinar con marca si es necesario
                if (!string.IsNullOrEmpty(modelo) && !poliza.Conmaraut?.Contains(modelo) == true)
                {
                    poliza.Conmaraut = $"{poliza.Conmaraut} {modelo}".Trim();
                }
            });

            ExtractFieldValue(fields, "vehiculo.año", value => {
                if (int.TryParse(LimpiarTexto(value), out int anio))
                {
                    poliza.Conanioaut = anio;

                    // Agregar año a descripción si no está presente
                    if (!string.IsNullOrEmpty(poliza.Conmaraut) && !poliza.Conmaraut.Contains($"({anio})"))
                    {
                        poliza.Conmaraut = $"{poliza.Conmaraut} ({anio})";
                    }
                }
            });

            ExtractFieldValue(fields, "vehiculo.matricula", value => {
                poliza.Conmataut = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "vehiculo.padron", value => {
                poliza.Conpadaut = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "vehiculo.motor", value => {
                poliza.Conmotor = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "vehiculo.chasis", value => {
                poliza.Conchasis = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "vehiculo.combustible", value => {
                poliza.Combustibles = LimpiarTexto(value);
            });

            // === FECHAS ===
            ExtractFieldValue(fields, "poliza.vigencia.desde", value => {
                var fecha = ParsearFecha(value);
                if (fecha.HasValue)
                {
                    poliza.Confchdes = fecha.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            });

            ExtractFieldValue(fields, "poliza.vigencia.hasta", value => {
                var fecha = ParsearFecha(value);
                if (fecha.HasValue)
                {
                    poliza.Confchhas = fecha.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            });

            ExtractFieldValue(fields, "poliza.fecha_emision", value => {
                var fechaEmision = ParsearFecha(value);
                if (fechaEmision.HasValue)
                {
                    poliza.Ingresado = fechaEmision.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            });

            // === DATOS FINANCIEROS ===
            ExtractFieldValue(fields, "financiero.prima_comercial", value => {
                poliza.Conpremio = ParsearMonto(value);
            });

            ExtractFieldValue(fields, "financiero.premio_total", value => {
                poliza.Contot = ParsearMonto(value);
            });

            ExtractFieldValue(fields, "financiero.impuesto_msp", value => {
                poliza.Conimp = ParsearMonto(value);
            });

            // === MONEDA ===
            ExtractFieldValue(fields, "poliza.moneda", value => {
                var moneda = LimpiarTexto(value)?.ToUpper();
                poliza.Moncod = DeterminarCodigoMoneda(moneda);
            });

            // === DATOS DEL CORREDOR ===
            ExtractFieldValue(fields, "corredor.nombre", value => {
                poliza.Corrnom = LimpiarTexto(value);
            });

            ExtractFieldValue(fields, "corredor.numero", value => {
                if (int.TryParse(LimpiarTexto(value), out int numero))
                {
                    poliza.Otrcorrcod = numero;
                }
            });

            // === DATOS DE PAGO ===
            ExtractFieldValue(fields, "pago.medio", value => {
                var medioPago = LimpiarTexto(value);
                // Mapear medio de pago si es necesario
                // Velneo podría tener códigos específicos para medios de pago
            });

            ExtractFieldValue(fields, "pago.cuotas", value => {
                if (int.TryParse(LimpiarTexto(value), out int cuotas))
                {
                    poliza.Concuo = cuotas;
                }
            });
        }

        #endregion

        #region Extracción desde Content (Fallback)

        private void ExtractFromContentFallback(VelneoPoliza poliza, string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            // Solo aplicar fallbacks si los campos principales están vacíos
            if (string.IsNullOrEmpty(poliza.Conpol))
            {
                var numeroPoliza = ExtraerNumeroPoliza(content);
                if (!string.IsNullOrEmpty(numeroPoliza))
                {
                    poliza.Conpol = numeroPoliza;
                }
            }

            if (string.IsNullOrEmpty(poliza.Clinom))
            {
                var asegurado = ExtraerAsegurado(content);
                if (!string.IsNullOrEmpty(asegurado))
                {
                    poliza.Clinom = asegurado;
                }
            }

            if (string.IsNullOrEmpty(poliza.Conmaraut))
            {
                var vehiculo = ExtraerDescripcionVehiculo(content);
                if (!string.IsNullOrEmpty(vehiculo))
                {
                    poliza.Conmaraut = vehiculo;
                }
            }

            if (poliza.Conpremio?.ToString() == "0" || poliza.Conpremio == null)
            {
                var prima = ParsearMonto(ExtraerPrimaComercial(content));
                if (prima > 0)
                {
                    poliza.Conpremio = prima;
                }
            }

            if (poliza.Contot?.ToString() == "0" || poliza.Contot == null)
            {
                var premio = ParsearMonto(ExtraerPremioTotal(content));
                if (premio > 0)
                {
                    poliza.Contot = premio;
                }
            }

            // Extraer fechas de vigencia si no están presentes
            if (string.IsNullOrEmpty(poliza.Confchdes) || string.IsNullOrEmpty(poliza.Confchhas))
            {
                var (fechaDesde, fechaHasta) = ExtraerVigencia(content);
                if (fechaDesde.HasValue && string.IsNullOrEmpty(poliza.Confchdes))
                {
                    poliza.Confchdes = fechaDesde.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                if (fechaHasta.HasValue && string.IsNullOrEmpty(poliza.Confchhas))
                {
                    poliza.Confchhas = fechaHasta.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            }
        }

        #endregion

        #region Métodos Auxiliares de Extracción

        private void ExtractFieldValue(JToken fields, string fieldName, Action<string> setter)
        {
            try
            {
                var field = fields[fieldName];
                if (field != null)
                {
                    var value = field["content"]?.ToString() ??
                               field["valueString"]?.ToString() ??
                               field["value"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        setter(value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error extracting field {FieldName}", fieldName);
            }
        }

        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            // Limpiar saltos de línea y espacios múltiples
            var limpio = texto.Replace("\n", " ").Replace("\r", "").Trim();
            limpio = Regex.Replace(limpio, @"\s+", " ");

            // Remover etiquetas comunes al inicio
            var etiquetas = new[]
            {
                "MARCA:", "AÑO:", "MODELO:", "MOTOR:", "CHASIS:", "MATRÍCULA:",
                "Asegurado:", "Nombre:", "Prima Comercial:", "Premio Total:",
                "Vigencia:", "Nº de Póliza:", "Número de Póliza:"
            };

            foreach (var etiqueta in etiquetas)
            {
                if (limpio.StartsWith(etiqueta, StringComparison.OrdinalIgnoreCase))
                {
                    limpio = limpio.Substring(etiqueta.Length).Trim();
                    break;
                }
            }

            return limpio.Trim(' ', ':', '-', '.', ',');
        }

        private DateTime? ParsearFecha(string fechaTexto)
        {
            if (string.IsNullOrWhiteSpace(fechaTexto)) return null;

            var fechaLimpia = LimpiarTexto(fechaTexto);

            // Intentar diferentes formatos de fecha
            var formatos = new[]
            {
                "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-dd",
                "dd/MM/yyyy HH:mm", "dd-MM-yyyy HH:mm",
                "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd HH:mm:ss"
            };

            foreach (var formato in formatos)
            {
                if (DateTime.TryParseExact(fechaLimpia, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    return fecha;
                }
            }

            // Intentar parsing automático como último recurso
            if (DateTime.TryParse(fechaLimpia, out var fechaAuto))
            {
                return fechaAuto;
            }

            return null;
        }

        private int ParsearMonto(string montoTexto)
        {
            if (string.IsNullOrWhiteSpace(montoTexto)) return 0;

            var montoLimpio = LimpiarTexto(montoTexto);

            // Remover símbolos de moneda y formateo
            montoLimpio = Regex.Replace(montoLimpio, @"[^\d.,\-]", "");
            montoLimpio = montoLimpio.Replace(".", "").Replace(",", ".");

            if (decimal.TryParse(montoLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out var monto))
            {
                return (int)(monto * 100); // Convertir a centavos
            }

            return 0;
        }

        private int DeterminarCodigoMoneda(string moneda)
        {
            if (string.IsNullOrEmpty(moneda)) return 1; // Default a peso uruguayo

            moneda = moneda.ToUpper();
            if (moneda.Contains("PESO") || moneda.Contains("UYU") || moneda.Contains("$U"))
                return 1;
            if (moneda.Contains("DOLAR") || moneda.Contains("USD") || moneda.Contains("US$"))
                return 2;

            return 1; // Default
        }

        #endregion

        #region Extractores de Content (Regex)

        private string ExtraerNumeroPoliza(string content)
        {
            // Buscar patrones como "Nº de Póliza: 9603235" o "9603235 / 0"
            var patrones = new[]
            {
                @"(?:Nº de Póliza|Número de Póliza|Póliza)\s*[:/]\s*(\d{6,8})",
                @"(\d{7,8})\s*/\s*\d+", // Formato número/endoso
                @"(?:Policy|POL)\s*[:#]\s*(\d{6,8})"
            };

            foreach (var patron in patrones)
            {
                var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return "";
        }

        private string ExtraerAsegurado(string content)
        {
            var patrones = new[]
            {
                @"Asegurado\s*:\s*([^\n\r]+)",
                @"ASEGURADO\s*:\s*([^\n\r]+)",
                @"Nombre\s*:\s*([^\n\r]+)",
                @"Cliente\s*:\s*([^\n\r]+)"
            };

            foreach (var patron in patrones)
            {
                var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            return "";
        }

        private string ExtraerDescripcionVehiculo(string content)
        {
            var patrones = new[]
            {
                @"CHEVROLET\s+[^\n\r]+(?:\(\d{4}\))?",
                @"(?:Marca|MARCA)\s*:\s*([^\n\r]+)",
                @"(?:Vehículo|VEHÍCULO)\s*:\s*([^\n\r]+)",
                @"Descripción del bien\s*:\s*VEHÍCULO\s+([^\n\r]+)"
            };

            foreach (var patron in patrones)
            {
                var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var vehiculo = match.Groups[match.Groups.Count - 1].Value.Trim();
                    if (vehiculo.Length > 5) // Validar que tenga contenido útil
                    {
                        return vehiculo;
                    }
                }
            }

            return "";
        }

        private string ExtraerPrimaComercial(string content)
        {
            var patron = @"Prima Comercial\s*:\s*\$\s*([\d,]+\.?\d*)";
            var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }

        private string ExtraerPremioTotal(string content)
        {
            var patrones = new[]
            {
                @"Premio Total\s*:\s*\$\s*([\d,]+\.?\d*)",
                @"Total a Pagar\s*:\s*\$\s*([\d,]+\.?\d*)",
                @"Prima\s*:\s*\$\s*([\d,]+\.?\d*)"
            };

            foreach (var patron in patrones)
            {
                var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            return "";
        }

        private (DateTime?, DateTime?) ExtraerVigencia(string content)
        {
            var patron = @"Vigencia\s*:\s*(\d{2}/\d{2}/\d{4})\s*-\s*(\d{2}/\d{2}/\d{4})";
            var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var fechaDesde = ParsearFecha(match.Groups[1].Value);
                var fechaHasta = ParsearFecha(match.Groups[2].Value);
                return (fechaDesde, fechaHasta);
            }

            return (null, null);
        }

        #endregion

        #region Validaciones y Completado

        private void CompletarCamposCalculados(VelneoPoliza poliza)
        {
            // Calcular premio total si no está presente pero tenemos prima comercial
            var premioActual = ConvertToInt(poliza.Contot);
            var primaActual = ConvertToInt(poliza.Conpremio);

            if (premioActual == 0 && primaActual > 0)
            {
                // Estimación básica: prima + ~22% (impuestos estimados)
                poliza.Contot = (int)(primaActual * 1.22m);
            }

            // Determinar sección basada en el tipo de vehículo/ramo
            if (poliza.Ramo?.ToUpper().Contains("AUTO") == true)
            {
                poliza.Seccod = 4; // Automóviles
            }

            // Agregar observaciones de procesamiento
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            if (string.IsNullOrEmpty(poliza.Observaciones))
            {
                poliza.Observaciones = $"Procesado automáticamente con Azure DI - {timestamp}";
            }
            else if (!poliza.Observaciones.Contains("Azure DI"))
            {
                poliza.Observaciones = $"{poliza.Observaciones} | Azure DI - {timestamp}";
            }
        }

        private void ValidarYCorregirDatos(VelneoPoliza poliza)
        {
            var advertencias = new List<string>();

            // Validar número de póliza
            if (string.IsNullOrEmpty(poliza.Conpol))
            {
                _logger?.LogWarning("Número de póliza no encontrado");
                advertencias.Add("Sin número de póliza");
            }

            // Validar asegurado
            if (string.IsNullOrEmpty(poliza.Clinom))
            {
                _logger?.LogWarning("Nombre del asegurado no encontrado");
                advertencias.Add("Sin nombre de asegurado");
            }

            // Validar vehículo
            if (string.IsNullOrEmpty(poliza.Conmaraut))
            {
                _logger?.LogWarning("Descripción del vehículo no encontrada");
                advertencias.Add("Sin descripción de vehículo");
            }

            // Validar montos
            if (ConvertToInt(poliza.Conpremio) == 0)
            {
                _logger?.LogWarning("Prima comercial no encontrada o es cero");
                advertencias.Add("Sin prima comercial");
            }

            // Validar fechas de vigencia
            if (string.IsNullOrEmpty(poliza.Confchdes) || string.IsNullOrEmpty(poliza.Confchhas))
            {
                _logger?.LogWarning("Fechas de vigencia incompletas");
                advertencias.Add("Fechas de vigencia incompletas");
            }

            // Agregar advertencias a observaciones
            if (advertencias.Count > 0)
            {
                var textoAdvertencias = string.Join(", ", advertencias);
                poliza.Observaciones = string.IsNullOrEmpty(poliza.Observaciones)
                    ? $"ADVERTENCIAS: {textoAdvertencias}"
                    : $"{poliza.Observaciones} | ADVERTENCIAS: {textoAdvertencias}";
            }

            // Truncar observaciones si son muy largas
            if (poliza.Observaciones?.Length > 500)
            {
                poliza.Observaciones = poliza.Observaciones.Substring(0, 497) + "...";
            }
        }

        private int ConvertToInt(object value)
        {
            if (value == null) return 0;
            if (value is int intValue) return intValue;
            if (int.TryParse(value.ToString(), out int result)) return result;
            return 0;
        }

        #endregion

        #region Método de compatibilidad para PolizaDto

        /// <summary>
        /// Método de compatibilidad para mantener la interfaz existente con PolizaDto
        /// </summary>
        public RegularizadorPolizas.Application.DTOs.PolizaDto ParseToPolizaDto(JObject azureResult)
        {
            var velneoPoliza = ParseToVelneoPoliza(azureResult);

            // Usar el mapper existente para convertir VelneoPoliza a PolizaDto
            return RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers.PolizaMappers.ToPolizaDto(velneoPoliza);
        }

        #endregion
    }
}