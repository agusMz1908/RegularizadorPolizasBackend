using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace RegularizadorPolizas.Application.Services
{
    public class VelneoDocumentResultParser
    {
        private readonly ILogger<VelneoDocumentResultParser>? _logger;

        public VelneoDocumentResultParser(ILogger<VelneoDocumentResultParser> logger = null)
        {
            _logger = logger;
        }

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

        public DateTime? ParsearFecha(string fechaTexto)
        {
            if (string.IsNullOrWhiteSpace(fechaTexto)) return null;

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
                    return fecha;
                }
            }

            if (DateTime.TryParse(fechaLimpia, out var fechaAuto))
            {
                return fechaAuto;
            }

            return null;
        }

        public int ParsearMonto(string montoTexto)
        {
            if (string.IsNullOrWhiteSpace(montoTexto)) return 0;

            var montoLimpio = LimpiarTexto(montoTexto);
            montoLimpio = Regex.Replace(montoLimpio, @"[^\d.,\-]", "");
            montoLimpio = montoLimpio.Replace(".", "").Replace(",", ".");

            if (decimal.TryParse(montoLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out var monto))
            {
                return (int)(monto * 100);
            }

            return 0;
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

        public int ExtraerPrimaComercial(string content)
        {
            var patron = @"Prima Comercial\s*:\s*\$\s*([\d,]+\.?\d*)";
            var match = Regex.Match(content, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return ParsearMonto(match.Groups[1].Value);
            }
            return 0;
        }

        public int ExtraerPremioTotal(string content)
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
            return 0;
        }

        public (DateTime?, DateTime?) ExtraerVigencia(string content)
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
    }
}
