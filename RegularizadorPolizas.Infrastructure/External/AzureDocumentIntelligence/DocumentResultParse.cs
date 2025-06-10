using System.Globalization;
using System.Text.RegularExpressions;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using RegularizadorPolizas.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace RegularizadorPolizas.Infrastructure.External
{
    public class DocumentResultParser
    {
        private readonly ILogger<DocumentResultParser>? _logger;

        // Constructor con logger
        public DocumentResultParser(ILogger<DocumentResultParser> logger)
        {
            _logger = logger;
        }

        // Constructor sin logger (para casos donde no esté disponible)
        public DocumentResultParser()
        {
            _logger = null;
        }

        // Método principal para Azure Document Intelligence
        public PolizaDto? ParseToPolizaDto(AnalyzeResult analyzeResult)
        {
            if (analyzeResult?.Documents == null || !analyzeResult.Documents.Any())
                return null;

            var poliza = new PolizaDto();
            var document = analyzeResult.Documents.FirstOrDefault();

            if (document?.Fields == null)
                return poliza;

            var fields = document.Fields;

            MapearDatosBasicos(poliza, fields);
            MapearDatosAsegurado(poliza, fields);
            MapearDatosVehiculo(poliza, fields);
            MapearDatosFinancieros(poliza, fields);
            MapearDatosCorredor(poliza, fields);

            return poliza;
        }

        // Método principal para DocumentResultDto - CORREGIDO
        public PolizaDto ParseToPolizaDto(DocumentResultDto documento)
        {
            if (documento?.CamposExtraidos == null)
            {
                _logger?.LogWarning("Document or extracted fields are null for document {DocumentId}", documento?.DocumentoId);
                return new PolizaDto
                {
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    Procesado = false
                };
            }

            var poliza = new PolizaDto
            {
                Activo = true,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Procesado = true,
                Convig = "1" // Activa por defecto
            };

            var campos = documento.CamposExtraidos;

            try
            {
                _logger?.LogDebug("Starting document mapping for document {DocumentId} with {FieldCount} fields",
                    documento.DocumentoId, campos.Count);

                // Mapear usando la misma lógica pero adaptada para Dictionary
                MapearDatosBasicosFromDict(poliza, campos);
                MapearDatosAseguradoFromDict(poliza, campos);
                MapearDatosVehiculoFromDict(poliza, campos);
                MapearDatosFinancierosFromDict(poliza, campos);
                MapearDatosCorredorFromDict(poliza, campos);

                _logger?.LogInformation("Successfully mapped document {DocumentId} to poliza. Policy: {PolicyNumber}, Client: {ClientName}",
                    documento.DocumentoId, poliza.Conpol, poliza.Clinom);

                return poliza;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error mapping document {DocumentId} to poliza", documento.DocumentoId);

                // En caso de error, devolver póliza básica para que no falle completamente
                poliza.Observaciones = $"Error en mapeo automático: {ex.Message}. Revisar manualmente.";
                return poliza;
            }
        }

        #region Mapeo desde AnalyzeResult (Azure Document Intelligence)

        private void MapearDatosBasicos(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
        {
            ExtraerCampo(fields, "poliza.numero", valor => poliza.Conpol = valor);
            ExtraerCampo(fields, "poliza.endoso", valor => poliza.Conend = valor);
            ExtraerCampo(fields, "poliza.ramo", valor => poliza.Ramo = valor);
            ExtraerCampo(fields, "poliza.producto", valor => poliza.Ramo = valor);
            ExtraerCampo(fields, "poliza.tipo_renovacion", valor => poliza.Congesti = valor);
            ExtraerCampo(fields, "poliza.moneda", valor => {
                if (valor?.ToLower().Contains("peso") == true)
                    poliza.Moncod = 1;
                else if (valor?.ToLower().Contains("dolar") == true)
                    poliza.Moncod = 2;
            });

            ExtraerFecha(fields, "poliza.vigencia.desde", fecha => poliza.Confchdes = fecha);
            ExtraerFecha(fields, "poliza.vigencia.hasta", fecha => poliza.Confchhas = fecha);
            ExtraerFecha(fields, "poliza.fecha_emision", fecha => poliza.FechaCreacion = fecha);
        }

        private void MapearDatosAsegurado(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
        {
            ExtraerCampo(fields, "asegurado.nombre", valor => poliza.Clinom = valor);
            ExtraerCampo(fields, "asegurado.direccion", valor => poliza.Condom = valor);
            ExtraerCampo(fields, "asegurado.localidad", valor => poliza.Clilocnom = valor);
            ExtraerCampo(fields, "asegurado.departamento", valor => poliza.Clidptnom = valor);
            ExtraerCampo(fields, "asegurado.documento.numero", valor => poliza.Cliruc = valor);
            ExtraerCampo(fields, "asegurado.radio", valor => poliza.Cliposcod = int.TryParse(valor, out int radio) ? radio : null);
        }

        private void MapearDatosVehiculo(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
        {
            ExtraerCampo(fields, "vehiculo.marca", valor => poliza.Conmaraut = valor);
            ExtraerCampo(fields, "vehiculo.tipo", valor => poliza.Contpocob = valor);
            ExtraerCampo(fields, "vehiculo.modelo", valor => poliza.Condetail = valor);
            ExtraerCampo(fields, "vehiculo.matricula", valor => poliza.Conmataut = valor);
            ExtraerCampo(fields, "vehiculo.motor", valor => poliza.Conmotor = valor);
            ExtraerCampo(fields, "vehiculo.padron", valor => poliza.Conpadaut = valor);
            ExtraerCampo(fields, "vehiculo.combustible", valor => poliza.Combustibles = valor);
            ExtraerCampo(fields, "vehiculo.año", valor => {
                if (int.TryParse(valor, out int anio))
                    poliza.Conanioaut = anio;
            });
        }

        private void MapearDatosFinancieros(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
        {
            ExtraerDecimal(fields, "financiero.prima_comercial", valor => poliza.Conpremio = valor);
            ExtraerDecimal(fields, "financiero.premio_total", valor => poliza.Contot = valor);
            ExtraerDecimal(fields, "financiero.impuesto_msp", valor => poliza.Conimp = valor);

            ExtraerCampo(fields, "pago.medio", valor => poliza.Consta = valor);
            ExtraerCampo(fields, "pago.modo_facturacion", valor => {
                var match = Regex.Match(valor ?? "", @"(\d+)\s*cuotas?");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
                    poliza.Concuo = cuotas;
            });
        }

        private void MapearDatosCorredor(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
        {
            ExtraerCampo(fields, "corredor.nombre", valor => poliza.Com_alias = valor);
            ExtraerCampo(fields, "corredor.numero", valor => {
                if (int.TryParse(valor, out int numero))
                    poliza.Corrnom = numero;
            });
        }

        #endregion

        #region Mapeo desde DocumentResultDto (Dictionary) - IMPLEMENTACIÓN COMPLETA

        private void MapearDatosBasicosFromDict(PolizaDto poliza, Dictionary<string, string> campos)
        {
            // Número de póliza - múltiples variaciones posibles
            if (TryGetCampo(campos, out var numeroPoliza,
                "poliza.numero", "numero_poliza", "policy_number", "poliza", "nro_poliza", "numero"))
            {
                poliza.Conpol = LimpiarTexto(numeroPoliza);
            }

            // Endoso
            if (TryGetCampo(campos, out var endoso,
                "poliza.endoso", "endoso", "endorsement", "nro_endoso"))
            {
                poliza.Conend = LimpiarTexto(endoso);
            }

            // Ramo
            if (TryGetCampo(campos, out var ramo,
                "poliza.ramo", "poliza.producto", "ramo", "branch", "tipo_seguro", "producto"))
            {
                poliza.Ramo = LimpiarTexto(ramo);
            }
            else
            {
                poliza.Ramo = "AUTO"; // Por defecto para seguros de vehículos
            }

            // Moneda
            if (TryGetCampo(campos, out var moneda,
                "poliza.moneda", "moneda", "currency", "divisa"))
            {
                var monedaId = MapearMoneda(moneda);
                if (monedaId.HasValue)
                {
                    poliza.Moncod = monedaId;
                }
            }
            else
            {
                poliza.Moncod = 1; // Por defecto UYU
            }

            // Fechas de vigencia
            if (TryGetCampo(campos, out var fechaDesde,
                "poliza.vigencia.desde", "fecha_desde", "vigencia_desde", "start_date", "fecha_inicio"))
            {
                if (TryParseDate(fechaDesde, out var desde))
                {
                    poliza.Confchdes = desde;
                }
            }

            if (TryGetCampo(campos, out var fechaHasta,
                "poliza.vigencia.hasta", "fecha_hasta", "vigencia_hasta", "end_date", "fecha_fin", "vencimiento"))
            {
                if (TryParseDate(fechaHasta, out var hasta))
                {
                    poliza.Confchhas = hasta;
                }
            }

            // Fecha de emisión
            if (TryGetCampo(campos, out var fechaEmision,
                "poliza.fecha_emision", "fecha_emision", "emission_date"))
            {
                if (TryParseDate(fechaEmision, out var emision))
                {
                    poliza.FechaCreacion = emision;
                }
            }

            // Validar y corregir fechas si es necesario
            ValidarYCorregirFechas(poliza);
        }

        private void MapearDatosAseguradoFromDict(PolizaDto poliza, Dictionary<string, string> campos)
        {
            // Nombre del asegurado
            if (TryGetCampo(campos, out var nombre,
                "asegurado.nombre", "cliente_nombre", "asegurado_nombre", "nombre_asegurado", "client_name", "nombre"))
            {
                poliza.Clinom = LimpiarTexto(nombre);
            }

            // Dirección
            if (TryGetCampo(campos, out var direccion,
                "asegurado.direccion", "cliente_direccion", "direccion", "domicilio", "address"))
            {
                poliza.Condom = LimpiarTexto(direccion);
            }

            // Localidad
            if (TryGetCampo(campos, out var localidad,
                "asegurado.localidad", "localidad", "ciudad", "city"))
            {
                poliza.Clilocnom = LimpiarTexto(localidad);
            }

            // Departamento
            if (TryGetCampo(campos, out var departamento,
                "asegurado.departamento", "departamento", "estado", "state", "provincia"))
            {
                poliza.Clidptnom = LimpiarTexto(departamento);
            }

            // Documento
            if (TryGetCampo(campos, out var documento,
                "asegurado.documento.numero", "cliente_documento", "documento", "cedula", "rut", "ci"))
            {
                poliza.Cliruc = LimpiarDocumento(documento);
            }

            // Radio/Código postal
            if (TryGetCampo(campos, out var radio,
                "asegurado.radio", "codigo_postal", "radio", "postal_code"))
            {
                if (int.TryParse(radio, out var radioNum))
                {
                    poliza.Cliposcod = radioNum;
                }
            }

            // Email
            if (TryGetCampo(campos, out var email,
                "asegurado.email", "cliente_email", "email", "correo"))
            {
                if (EsEmailValido(email))
                {
                    poliza.Cliemail = LimpiarTexto(email);
                }
            }

            // Teléfono
            if (TryGetCampo(campos, out var telefono,
                "asegurado.telefono", "cliente_telefono", "telefono", "phone", "celular"))
            {
                poliza.Clitelcel = LimpiarTelefono(telefono);
            }
        }

        private void MapearDatosVehiculoFromDict(PolizaDto poliza, Dictionary<string, string> campos)
        {
            // Marca
            if (TryGetCampo(campos, out var marca,
                "vehiculo.marca", "vehiculo_marca", "marca", "brand", "vehicle_brand"))
            {
                poliza.Conmaraut = LimpiarTexto(marca);
            }

            // Tipo de vehículo
            if (TryGetCampo(campos, out var tipo,
                "vehiculo.tipo", "vehiculo_tipo", "tipo", "vehicle_type"))
            {
                poliza.Contpocob = LimpiarTexto(tipo);
            }

            // Modelo
            if (TryGetCampo(campos, out var modelo,
                "vehiculo.modelo", "vehiculo_modelo", "modelo", "model"))
            {
                poliza.Condetail = LimpiarTexto(modelo);
            }

            // Matrícula
            if (TryGetCampo(campos, out var matricula,
                "vehiculo.matricula", "vehiculo_matricula", "matricula", "placa", "plate", "patente"))
            {
                poliza.Conmataut = LimpiarMatricula(matricula);
            }

            // Motor
            if (TryGetCampo(campos, out var motor,
                "vehiculo.motor", "vehiculo_motor", "motor", "engine", "numero_motor"))
            {
                poliza.Conmotor = LimpiarTexto(motor);
            }

            // Padrón
            if (TryGetCampo(campos, out var padron,
                "vehiculo.padron", "vehiculo_padron", "padron", "numero_padron"))
            {
                poliza.Conpadaut = LimpiarTexto(padron);
            }

            // Combustible
            if (TryGetCampo(campos, out var combustible,
                "vehiculo.combustible", "combustible", "fuel", "tipo_combustible"))
            {
                poliza.Combustibles = LimpiarTexto(combustible);
            }

            // Año
            if (TryGetCampo(campos, out var año,
                "vehiculo.año", "vehiculo_año", "año", "year", "modelo_año"))
            {
                if (int.TryParse(año, out var anio) && anio >= 1900 && anio <= DateTime.Now.Year + 2)
                {
                    poliza.Conanioaut = anio;
                }
            }

            // Chasis
            if (TryGetCampo(campos, out var chasis,
                "vehiculo.chasis", "chasis", "chassis", "numero_chasis", "vin"))
            {
                poliza.Conchasis = LimpiarTexto(chasis);
            }
        }

        private void MapearDatosFinancierosFromDict(PolizaDto poliza, Dictionary<string, string> campos)
        {
            // Prima comercial
            if (TryGetCampo(campos, out var primaComercial,
                "financiero.prima_comercial", "prima_comercial", "premium", "prima"))
            {
                if (TryParseDecimalDict(primaComercial, out var prima))
                {
                    poliza.Conpremio = prima;
                }
            }

            // Premio total
            if (TryGetCampo(campos, out var premioTotal,
                "financiero.premio_total", "premio_total", "total", "total_amount"))
            {
                if (TryParseDecimalDict(premioTotal, out var premio))
                {
                    poliza.Contot = premio;

                    // Si no hay prima comercial, usar el premio total
                    if (!poliza.Conpremio.HasValue)
                    {
                        poliza.Conpremio = premio;
                    }
                }
            }

            // Impuesto MSP
            if (TryGetCampo(campos, out var impuesto,
                "financiero.impuesto_msp", "impuestos", "tax", "impuesto", "iva"))
            {
                if (TryParseDecimalDict(impuesto, out var imp))
                {
                    poliza.Conimp = imp;
                }
            }

            // Medio de pago
            if (TryGetCampo(campos, out var medioPago,
                "pago.medio", "medio_pago", "payment_method", "forma_pago"))
            {
                poliza.Consta = LimpiarTexto(medioPago);
            }

            // Modo de facturación / Cuotas
            if (TryGetCampo(campos, out var modoFacturacion,
                "pago.modo_facturacion", "cuotas", "installments", "numero_cuotas"))
            {
                var match = Regex.Match(modoFacturacion ?? "", @"(\d+)\s*cuotas?", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
                {
                    poliza.Concuo = cuotas;
                }
                else if (int.TryParse(modoFacturacion, out int cuotasDirecto))
                {
                    poliza.Concuo = cuotasDirecto;
                }
            }
        }

        private void MapearDatosCorredorFromDict(PolizaDto poliza, Dictionary<string, string> campos)
        {
            // Nombre del corredor
            if (TryGetCampo(campos, out var nombreCorredor,
                "corredor.nombre", "corredor", "broker", "intermediario", "corredor_nombre"))
            {
                poliza.Com_alias = LimpiarTexto(nombreCorredor);
            }

            // Número del corredor
            if (TryGetCampo(campos, out var numeroCorredor,
                "corredor.numero", "corredor_numero", "broker_number", "numero_corredor"))
            {
                if (int.TryParse(numeroCorredor, out int numero))
                {
                    poliza.Corrnom = numero;
                }
            }
        }

        #endregion

        #region Métodos Helper para Azure Fields

        private void ExtraerCampo(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, Action<string> asignar)
        {
            if (fields.TryGetValue(fieldName, out var field) &&
                !string.IsNullOrWhiteSpace(field.Content))
            {
                asignar(field.Content.Trim());
            }
        }

        private void ExtraerFecha(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, Action<DateTime> asignar)
        {
            ExtraerCampo(fields, fieldName, valor => {
                if (DateTime.TryParse(valor, out DateTime fecha))
                    asignar(fecha);
            });
        }

        private void ExtraerDecimal(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, Action<decimal> asignar)
        {
            ExtraerCampo(fields, fieldName, valor => {
                string valorLimpio = valor.Replace("$", "").Replace(",", "").Replace(".", ",").Trim();
                if (decimal.TryParse(valorLimpio, NumberStyles.Any, new CultureInfo("es-UY"), out decimal resultado))
                    asignar(resultado);
            });
        }

        #endregion

        #region Métodos Helper para Dictionary

        private bool TryGetCampo(Dictionary<string, string> campos, out string valor, params string[] claves)
        {
            valor = string.Empty;

            foreach (var clave in claves)
            {
                // Buscar clave exacta
                if (campos.TryGetValue(clave, out valor) && !string.IsNullOrWhiteSpace(valor))
                {
                    return true;
                }

                // Buscar clave insensible a mayúsculas
                var claveEncontrada = campos.Keys.FirstOrDefault(k =>
                    k.Equals(clave, StringComparison.OrdinalIgnoreCase));

                if (claveEncontrada != null && !string.IsNullOrWhiteSpace(campos[claveEncontrada]))
                {
                    valor = campos[claveEncontrada];
                    return true;
                }
            }

            return false;
        }

        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return texto.Trim()
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Replace("\t", " ")
                        .Trim();
        }

        private string LimpiarDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return string.Empty;

            return documento.Replace(".", "")
                           .Replace("-", "")
                           .Replace(" ", "")
                           .Trim();
        }

        private string LimpiarMatricula(string matricula)
        {
            if (string.IsNullOrWhiteSpace(matricula))
                return string.Empty;

            return matricula.Replace(" ", "")
                           .Replace("-", "")
                           .ToUpperInvariant()
                           .Trim();
        }

        private string LimpiarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return string.Empty;

            return telefono.Replace(" ", "")
                          .Replace("-", "")
                          .Replace("(", "")
                          .Replace(")", "")
                          .Replace("+", "")
                          .Trim();
        }

        private bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool TryParseDecimalDict(string texto, out decimal valor)
        {
            valor = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return false;

            // Limpiar texto para parsing
            var textoLimpio = texto.Replace("$", "")
                                  .Replace("UYU", "")
                                  .Replace("USD", "")
                                  .Replace(",", "")
                                  .Trim();

            return decimal.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out valor);
        }

        private bool TryParseDate(string texto, out DateTime fecha)
        {
            fecha = DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(texto))
                return false;

            // Intentar varios formatos de fecha comunes
            var formatos = new[]
            {
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "yyyy-MM-dd",
                "dd-MM-yyyy",
                "d/M/yyyy",
                "d-M-yyyy",
                "yyyy/MM/dd"
            };

            foreach (var formato in formatos)
            {
                if (DateTime.TryParseExact(texto.Trim(), formato, null,
                    DateTimeStyles.None, out fecha))
                {
                    return true;
                }
            }

            // Último intento con parsing automático
            return DateTime.TryParse(texto, out fecha);
        }

        private int? MapearMoneda(string moneda)
        {
            if (string.IsNullOrWhiteSpace(moneda))
                return null;

            var monedaNormalizada = moneda.ToUpperInvariant().Trim();

            return monedaNormalizada switch
            {
                "UYU" or "PESO" or "PESOS" or "URUGUAYO" or "$U" => 1,
                "USD" or "DOLLAR" or "DOLAR" or "DOLARES" or "US$" => 2,
                "EUR" or "EURO" or "EUROS" or "€" => 3,
                "ARS" or "PESO ARGENTINO" or "PESOS ARGENTINOS" => 4,
                "BRL" or "REAL" or "REALES" or "BRASILEÑO" => 5,
                _ => 1 // Por defecto UYU
            };
        }

        private void ValidarYCorregirFechas(PolizaDto poliza)
        {
            // Si no hay fecha desde, usar hoy
            if (!poliza.Confchdes.HasValue)
            {
                poliza.Confchdes = DateTime.Today;
            }

            // Si no hay fecha hasta, usar un año después de fecha desde
            if (!poliza.Confchhas.HasValue && poliza.Confchdes.HasValue)
            {
                poliza.Confchhas = poliza.Confchdes.Value.AddYears(1);
            }

            // Validar coherencia de fechas
            if (poliza.Confchdes.HasValue && poliza.Confchhas.HasValue)
            {
                if (poliza.Confchdes >= poliza.Confchhas)
                {
                    _logger?.LogWarning("Invalid date range detected. Start: {Start}, End: {End}",
                        poliza.Confchdes, poliza.Confchhas);

                    // Corregir automáticamente
                    poliza.Confchhas = poliza.Confchdes.Value.AddYears(1);
                    poliza.Observaciones = (poliza.Observaciones ?? "") + "Fechas corregidas automáticamente. ";
                }
            }
        }

        #endregion
    }
}