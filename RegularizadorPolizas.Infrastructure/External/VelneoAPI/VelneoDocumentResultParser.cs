using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI
{
    public class VelneoDocumentResultParser
    {
        private readonly ILogger<VelneoDocumentResultParser>? _logger;

        public VelneoDocumentResultParser(ILogger<VelneoDocumentResultParser> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Método principal para parsear hacia PolizaDto (para compatibilidad)
        /// </summary>
        public PolizaDto ParseToPolizaDto(JObject azureResult)
        {
            try
            {
                var poliza = new PolizaDto
                {
                    // Campos básicos por defecto
                    Comcod = 1,
                    Seccod = 4,
                    Moncod = 1,
                    Convig = "1",
                    Consta = "1",
                    Contra = "2",
                    Ramo = "AUTOMOVILES",

                    // ✅ CORREGIDO: Fechas como string
                    Last_update = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Ingresado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Update_date = DateTime.Now.ToString("yyyy-MM-dd"),
                    FechaCreacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FechaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                    Activo = true,
                    Procesado = true
                };

                var analyzeResult = azureResult["analyzeResult"];
                if (analyzeResult == null) return poliza;

                // Extraer desde content como fallback
                var content = analyzeResult["content"]?.ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    ExtractFromContentToPolizaDto(poliza, content);
                }

                // Agregar observaciones
                poliza.Observaciones = $"Procesado automáticamente - {DateTime.Now:yyyy-MM-dd HH:mm}";

                _logger?.LogInformation("Successfully parsed document to PolizaDto: Policy={PolicyNumber}, Client={ClientName}",
                    poliza.Conpol, poliza.Clinom);

                return poliza;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error parsing Azure Document Intelligence result");

                return new PolizaDto
                {
                    Observaciones = $"Error en parsing: {ex.Message}",
                    Activo = true,
                    Procesado = false,

                    // ✅ CORREGIDO: Fechas como string en caso de error
                    FechaCreacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FechaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Ingresado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Last_update = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Update_date = DateTime.Now.ToString("yyyy-MM-dd")
                };
            }
        }

        #region Métodos de extracción desde content

        private void ExtractFromContentToPolizaDto(PolizaDto poliza, string content)
        {
            // Datos básicos de la póliza
            poliza.Conpol = ExtraerNumeroPoliza(content);
            poliza.Clinom = ExtraerAsegurado(content);
            poliza.Conmaraut = ExtraerDescripcionVehiculo(content);

            // Montos
            poliza.Conpremio = ExtraerPrimaComercial(content);
            poliza.Contot = ExtraerPremioTotal(content);

            // ✅ CORREGIDO: Fechas de vigencia como string
            var (fechaDesde, fechaHasta) = ExtraerVigenciaComoString(content);
            poliza.Confchdes = fechaDesde;
            poliza.Confchhas = fechaHasta;

            // Motor y chasis (si están disponibles)
            poliza.Conmotor = ExtraerMotor(content);
            poliza.Conchasis = ExtraerChasis(content);

            // Completar campos calculados
            if ((poliza.Contot ?? 0) == 0 && (poliza.Conpremio ?? 0) > 0)
            {
                poliza.Contot = (decimal)(poliza.Conpremio * 1.22m); // Prima + ~22% impuestos
            }
        }

        #endregion

        #region Métodos de limpieza y utilidad

        public string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            var limpio = texto.Replace("\n", " ").Replace("\r", "").Trim();
            limpio = Regex.Replace(limpio, @"\s+", " ");

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

        /// <summary>
        /// ✅ CORREGIDO: Parsear fecha y devolver como string
        /// </summary>
        public string ParsearFechaComoString(string fechaTexto)
        {
            if (string.IsNullOrWhiteSpace(fechaTexto)) return "";

            var fechaLimpia = LimpiarTexto(fechaTexto);

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
                    return fecha.ToString("yyyy-MM-dd");
                }
            }

            if (DateTime.TryParse(fechaLimpia, out var fechaAuto))
            {
                return fechaAuto.ToString("yyyy-MM-dd");
            }

            // Si no se puede parsear, devolver string vacío
            return "";
        }

        /// <summary>
        /// ✅ LEGACY: Mantener para compatibilidad, pero devolver DateTime? 
        /// </summary>
        public DateTime? ParsearFecha(string fechaTexto)
        {
            var fechaString = ParsearFechaComoString(fechaTexto);
            if (string.IsNullOrEmpty(fechaString))
                return null;

            if (DateTime.TryParse(fechaString, out var fecha))
                return fecha;

            return null;
        }

        /// <summary>
        /// ✅ CORREGIDO: Cambiar tipo de retorno de int a decimal?
        /// </summary>
        public decimal? ParsearMonto(string montoTexto)
        {
            if (string.IsNullOrWhiteSpace(montoTexto)) return null;

            var montoLimpio = LimpiarTexto(montoTexto);
            montoLimpio = Regex.Replace(montoLimpio, @"[^\d.,\-]", "");
            montoLimpio = montoLimpio.Replace(".", "").Replace(",", ".");

            if (decimal.TryParse(montoLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out var monto))
            {
                return monto;
            }

            return null;
        }

        public int DeterminarCodigoMoneda(string moneda)
        {
            if (string.IsNullOrEmpty(moneda)) return 1;

            moneda = moneda.ToUpper();
            if (moneda.Contains("PESO") || moneda.Contains("UYU") || moneda.Contains("$U"))
                return 1;
            if (moneda.Contains("DOLAR") || moneda.Contains("USD") || moneda.Contains("US$"))
                return 2;

            return 1;
        }

        #endregion

        #region Extractores específicos por regex

        public string ExtraerNumeroPoliza(string content)
        {
            var patrones = new[]
            {
                @"(?:Nº de Póliza|Número de Póliza|Póliza)\s*[:/]\s*(\d{6,8})",
                @"(\d{7,8})\s*/\s*\d+",
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

        public string ExtraerAsegurado(string content)
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

        public string ExtraerDescripcionVehiculo(string content)
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
                    if (vehiculo.Length > 5)
                    {
                        return vehiculo;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// ✅ CORREGIDO: Cambiar tipo de retorno de int a decimal?
        /// </summary>
        public decimal? ExtraerPrimaComercial(string content)
        {
            var patron = @"Prima Comercial\s*:\s*\$\s*([\d,]+\.?\d*)";
            var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return ParsearMonto(match.Groups[1].Value);
            }
            return null;
        }

        /// <summary>
        /// ✅ CORREGIDO: Cambiar tipo de retorno de int a decimal?
        /// </summary>
        public decimal? ExtraerPremioTotal(string content)
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
                    return ParsearMonto(match.Groups[1].Value);
                }
            }
            return null;
        }

        /// <summary>
        /// ✅ CORREGIDO: Devolver fechas como string
        /// </summary>
        public (string, string) ExtraerVigenciaComoString(string content)
        {
            var patron = @"Vigencia\s*:\s*(\d{2}/\d{2}/\d{4})\s*-\s*(\d{2}/\d{2}/\d{4})";
            var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var fechaDesde = ParsearFechaComoString(match.Groups[1].Value);
                var fechaHasta = ParsearFechaComoString(match.Groups[2].Value);
                return (fechaDesde, fechaHasta);
            }

            return ("", "");
        }

        /// <summary>
        /// ✅ LEGACY: Mantener para compatibilidad, pero usar ExtraerVigenciaComoString
        /// </summary>
        public (DateTime?, DateTime?) ExtraerVigencia(string content)
        {
            var (fechaDesdeStr, fechaHastaStr) = ExtraerVigenciaComoString(content);

            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            if (!string.IsNullOrEmpty(fechaDesdeStr) && DateTime.TryParse(fechaDesdeStr, out var desde))
                fechaDesde = desde;

            if (!string.IsNullOrEmpty(fechaHastaStr) && DateTime.TryParse(fechaHastaStr, out var hasta))
                fechaHasta = hasta;

            return (fechaDesde, fechaHasta);
        }

        public string ExtraerMotor(string content)
        {
            var patrones = new[]
            {
                @"Motor\s*[:#]\s*([A-Z0-9]+)",
                @"MOTOR\s*[:#]\s*([A-Z0-9]+)",
                @"Nº Motor\s*[:#]\s*([A-Z0-9]+)"
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

        public string ExtraerChasis(string content)
        {
            var patrones = new[]
            {
                @"Chasis\s*[:#]\s*([A-Z0-9]+)",
                @"CHASIS\s*[:#]\s*([A-Z0-9]+)",
                @"Nº Chasis\s*[:#]\s*([A-Z0-9]+)"
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

        #endregion
    }
}