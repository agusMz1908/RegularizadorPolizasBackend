using System.Globalization;
using System.Text.RegularExpressions;
using RegularizadorPolizas.Application.DTOs;
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

        public PolizaDto ParseToPolizaDto(JObject azureResult)
        {
            var poliza = new PolizaDto
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
                Last_update = DateTime.Now,
                Ingresado = DateTime.Now,
                Enviado = false,
                Leer = false
            };

            try
            {
                var analyzeResult = azureResult["analyzeResult"];
                if (analyzeResult == null) return poliza;

                // Estrategia 1: Extraer de documents (entidades del modelo custom)
                var documents = analyzeResult["documents"] as JArray;
                if (documents?.Count > 0)
                {
                    ExtractFromDocuments(poliza, documents[0]);
                }

                // Estrategia 2: Fallbacks desde content si es necesario
                var content = analyzeResult["content"]?.ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    ExtractFromContentFallback(poliza, content);
                }

                // Completar campos calculados y validaciones
                CompleteCalculatedFields(poliza);

                _logger?.LogInformation("Successfully parsed document to PolizaDto for Velneo: Policy={PolicyNumber}, Client={ClientName}",
                    poliza.Conpol, poliza.Clinom);
                return poliza;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error parsing Azure Document Intelligence result for Velneo");
                poliza.Observaciones = $"Error en parsing: {ex.Message}";
                return poliza;
            }
        }

        #region Extracción desde Documents (Modelo Custom Azure)

        private void ExtractFromDocuments(PolizaDto poliza, JToken document)
        {
            var fields = document["fields"];
            if (fields == null) return;

            ExtractFieldValue(fields, "poliza.numero", value => {
                poliza.Conpol = ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "poliza.endoso", value => {
                poliza.Conend = ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "poliza.fecha_emision", value => {
                var match = Regex.Match(value, @"(\d{2}/\d{2}/\d{4})");
                if (match.Success && DateTime.TryParseExact(match.Groups[1].Value, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    poliza.Ingresado = fecha;
                }
            });

            ExtractFieldValue(fields, "poliza.vigencia.desde", value => {
                var match = Regex.Match(value, @"(\d{2}/\d{2}/\d{4})");
                if (match.Success && DateTime.TryParseExact(match.Groups[1].Value, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    poliza.Confchdes = fecha;  
                }
            });

            ExtractFieldValue(fields, "poliza.vigencia.hasta", value => {
                if (DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    poliza.Confchhas = fecha;  
                }
            });

            ExtractFieldValue(fields, "poliza.moneda", value => {
                if (value.Contains("PESO") || value.Contains("UYU"))
                    poliza.Moncod = 1;
                else if (value.Contains("DOLAR") || value.Contains("USD"))
                    poliza.Moncod = 2;
            });

            ExtractFieldValue(fields, "poliza.tipo_renovacion", value => {
                if (value.Contains("AUTOMÁTICA"))
                    poliza.Congesti = "1"; // Automática
                else
                    poliza.Congesti = "2"; // Manual
            });

            ExtractFieldValue(fields, "asegurado.nombre", value => {
                poliza.Clinom = ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "asegurado.documento.numero", value => {
                // En Velneo podría ir en un campo específico de cliente
                var documento = ExtractCleanText(value);
                if (!string.IsNullOrEmpty(documento))
                {
                    poliza.Observaciones = (poliza.Observaciones ?? "") + $"CI: {documento}\n";
                }
            });

            ExtractFieldValue(fields, "asegurado.direccion", value => {
                poliza.Condom = ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "asegurado.radio", value => {
                var match = Regex.Match(value, @"Radio:\s*(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int radio))
                {
                    poliza.Observaciones = (poliza.Observaciones ?? "") + $"Radio: {radio}\n";
                }
            });

            ExtractFieldValue(fields, "vehiculo.marca", value => {
                var match = Regex.Match(value, @"MARCA\s*\n?\s*(.+)");
                var marca = match.Success ? match.Groups[1].Value.Trim() : ExtractCleanText(value);

                poliza.Conmaraut = marca;
            });

            ExtractFieldValue(fields, "vehiculo.modelo", value => {
                var match = Regex.Match(value, @"MODELO\s*\n?\s*(.+)");
                var modelo = match.Success ? match.Groups[1].Value.Trim() : ExtractCleanText(value);

                if (!string.IsNullOrEmpty(modelo))
                {
                    if (!string.IsNullOrEmpty(poliza.Conmaraut))
                        poliza.Conmaraut += " " + modelo;
                    else
                        poliza.Conmaraut = modelo;
                }
            });

            ExtractFieldValue(fields, "vehiculo.anio", value => {
                var match = Regex.Match(value, @"AÑO\s*\n?\s*(\d{4})");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int anio))
                {
                    poliza.Conanioaut = anio;
                }
            });

            ExtractFieldValue(fields, "vehiculo.matricula", value => {
                var match = Regex.Match(value, @"MATRÍCULA[.\s]*\n?\s*([A-Z0-9]+)");
                poliza.Conmataut = match.Success ? match.Groups[1].Value : ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "vehiculo.padron", value => {
                var match = Regex.Match(value, @"PADRÓN[.\s]*\n?\s*(\d+)");
                poliza.Conpadaut = match.Success ? match.Groups[1].Value : ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "vehiculo.motor", value => {
                var match = Regex.Match(value, @"MOTOR\s*\n?\s*([A-Z0-9]+)");
                poliza.Conmotor = match.Success ? match.Groups[1].Value : ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "vehiculo.chasis", value => {
                var match = Regex.Match(value, @"CHASIS\s*\n?\s*([A-Z0-9]+)");
                poliza.Conchasis = match.Success ? match.Groups[1].Value : ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "vehiculo.combustible", value => {
                var match = Regex.Match(value, @"COMBUSTIBLE\s*\n?\s*(.+)");
                var combustible = match.Success ? match.Groups[1].Value.Trim() : ExtractCleanText(value);

                // Mapear a códigos de Velneo
                if (combustible.Contains("NAFTA"))
                    poliza.Combustibles = "NAF";
                else if (combustible.Contains("GASOIL") || combustible.Contains("DIESEL"))
                    poliza.Combustibles = "GAS";
                else if (combustible.Contains("GAS"))
                    poliza.Combustibles = "GAS";
                else
                    poliza.Combustibles = combustible.Substring(0, Math.Min(3, combustible.Length));
            });

            ExtractFieldValue(fields, "vehiculo.tipo_vehiculo", value => {
                var match = Regex.Match(value, @"TIPO\s*DE\s*VEHÍCULO\s*\n?\s*(.+)");
                poliza.Contpocob = match.Success ? match.Groups[1].Value.Trim() : ExtractCleanText(value);
            });

            ExtractFieldValue(fields, "financiero.prima_comercial", value => {
                var amount = ExtractMoneyAmount(value);
                if (amount.HasValue)
                {
                    poliza.Conpremio = (int)Math.Round(amount.Value); 
                }
            });

            ExtractFieldValue(fields, "financiero.premio_total", value => {
                var amount = ExtractMoneyAmount(value);
                if (amount.HasValue)
                {
                    poliza.Contot = (int)Math.Round(amount.Value); 
                }
            });

            ExtractFieldValue(fields, "pago.modo_facturacion", value => {
                var match = Regex.Match(value, @"(\d+)\s*cuotas?");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
                {
                    poliza.Concuo = cuotas;
                }
            });

            ExtractFieldValue(fields, "pago.medio", value => {
                var match = Regex.Match(value, @"Medio\s*de\s*Pago:\s*\n?\s*(.+)");
                var medio = match.Success ? match.Groups[1].Value.Trim() : ExtractCleanText(value);

                if (medio.Contains("CAJA") || medio.Contains("EFECTIVO"))
                    poliza.Consta = "1";
                else if (medio.Contains("BANCO") || medio.Contains("TRANSFERENCIA"))
                    poliza.Consta = "2";
                else
                    poliza.Consta = "1";
            });

            ExtractFieldValue(fields, "corredor.nombre", value => {
                var match = Regex.Match(value, @"Nombre:\s*\n?\s*(.+)");
                var corredor = match.Success ? match.Groups[1].Value.Trim() : ExtractCleanText(value);
                poliza.Com_alias = corredor;
            });

            ExtractFieldValue(fields, "corredor.numero", value => {
                var match = Regex.Match(value, @"Número:\s*\n?\s*(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int numero))
                {
                    poliza.Corrnom = numero;
                }
            });
        }

        private void ExtractFieldValue(JToken fields, string fieldName, Action<string> setValue)
        {
            var field = fields[fieldName];
            if (field == null) return;

            var content = field["content"]?.ToString() ?? field["valueString"]?.ToString();
            if (!string.IsNullOrWhiteSpace(content))
            {
                setValue(content);
            }
        }

        #endregion

        #region Fallbacks desde Content

        private void ExtractFromContentFallback(PolizaDto poliza, string content)
        {
            if (string.IsNullOrEmpty(poliza.Conpol))
            {
                var polizaMatch = Regex.Match(content, @"(?:Ramo\s*/\s*Póliza\s*/\s*Endoso:\s*\d+\s*/\s*|Nº\s*de\s*Póliza\s*/\s*Endoso:\s*)(\d+)");
                if (polizaMatch.Success)
                {
                    poliza.Conpol = polizaMatch.Groups[1].Value;
                }
            }

            if (string.IsNullOrEmpty(poliza.Clinom))
            {
                var nombreMatch = Regex.Match(content, @"Asegurado:\s*\n\s*([A-ZÁÉÍÓÚÑ\s]+)");
                if (nombreMatch.Success)
                {
                    poliza.Clinom = ExtractCleanText(nombreMatch.Groups[1].Value);
                }
            }
        }

        #endregion

        #region Completar Campos Calculados

        private void CompleteCalculatedFields(PolizaDto poliza)
        {
            var detalles = new List<string>();

            if (!string.IsNullOrEmpty(poliza.Conmaraut))
                detalles.Add(poliza.Conmaraut);

            if (!string.IsNullOrEmpty(poliza.Conmataut))
                detalles.Add(poliza.Conmataut);

            if (poliza.Conanioaut > 0)
                detalles.Add(poliza.Conanioaut.ToString());

            if (poliza.Concuo > 0)
                detalles.Add($"CUOTAS: {poliza.Concuo}");

            if (poliza.Contot > 0)
                detalles.Add($"TOTAL: {poliza.Contot:F2}");

            poliza.Condetail = string.Join("   ", detalles);

            if (string.IsNullOrEmpty(poliza.Conpol))
            {
                _logger?.LogWarning("No se pudo extraer el número de póliza");
                poliza.Observaciones = (poliza.Observaciones ?? "") + "ADVERTENCIA: Número de póliza no detectado\n";
            }

            if (string.IsNullOrEmpty(poliza.Clinom))
            {
                _logger?.LogWarning("No se pudo extraer el nombre del asegurado");
                poliza.Observaciones = (poliza.Observaciones ?? "") + "ADVERTENCIA: Nombre del asegurado no detectado\n";
            }

            if (!string.IsNullOrEmpty(poliza.Conpol) && !string.IsNullOrEmpty(poliza.Clinom) &&
                (poliza.Observaciones?.Contains("ADVERTENCIA") != true))
            {
                poliza.Observaciones = "Póliza procesada automáticamente via Document Intelligence\n" + (poliza.Observaciones ?? "");
            }

            if (poliza.Confchdes == null || poliza.Confchdes == default(DateTime))
            {
                poliza.Confchdes = DateTime.Now;
            }

            if (poliza.Confchhas == null || poliza.Confchhas == default(DateTime))
            {
                poliza.Confchhas = (poliza.Confchdes ?? DateTime.Now).AddYears(1);
            }
        }

        #endregion

        #region Utilidades

        private string ExtractCleanText(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return texto.Trim()
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Replace("\t", " ")
                        .Replace("  ", " ")
                        .Trim();
        }

        private decimal? ExtractMoneyAmount(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            // Buscar patrones como "$ 17.054,47" o "$21.223,00"
            var match = Regex.Match(text, @"\$\s*([\d,.]+)");
            if (match.Success)
            {
                var amountStr = match.Groups[1].Value;

                if (amountStr.Contains(".") && amountStr.Contains(","))
                {
                    amountStr = amountStr.Replace(".", "").Replace(",", ".");
                }
                else if (amountStr.Contains(",") && !amountStr.Contains("."))
                {
                    amountStr = amountStr.Replace(",", ".");
                }
                else if (amountStr.Contains("."))
                {
                    var parts = amountStr.Split('.');
                    if (parts.Length > 1 && parts[parts.Length - 1].Length == 2)
                    {
                        amountStr = string.Join("", parts.Take(parts.Length - 1)) + "." + parts.Last();
                    }
                    else
                    {
                        amountStr = amountStr.Replace(".", "");
                    }
                }

                if (decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                {
                    return amount;
                }
            }

            return null;
        }

        #endregion
    }
}