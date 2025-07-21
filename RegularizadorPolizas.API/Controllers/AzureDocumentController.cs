using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs.Azure;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence.SmartDocumentParser;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AzureDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureDocumentController> _logger;
        private readonly SmartDocumentParser _smartParser;

        public AzureDocumentController(
            IConfiguration configuration,
            ILogger<AzureDocumentController> logger,
            SmartDocumentParser smartParser)
        {
            _configuration = configuration;
            _logger = logger;
            _smartParser = smartParser;
        }

        [HttpPost("process")]
        [Authorize]
        [ProducesResponseType(typeof(AzureBatchResponseDto), 200)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 400)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 500)]
        public async Task<ActionResult> ProcessDocument([Required] IFormFile file)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(AzureErrorResponseDto.ArchivoInvalido(file?.FileName));
                }

                _logger.LogInformation("🔄 PROCESAMIENTO PRINCIPAL: {FileName}", file.FileName);

                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                var client = new DocumentIntelligenceClient(
                    new Uri(endpoint),
                    new AzureKeyCredential(apiKey));

                using var stream = file.OpenReadStream();
                var binaryData = BinaryData.FromStream(stream);

                _logger.LogInformation("📄 PASO 1: Extrayendo campos raw del PDF...");
                var operation = await client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    modelId,
                    binaryData);

                var analyzeResult = operation.Value;
                var camposRaw = new Dictionary<string, string>();

                if (analyzeResult.Documents?.Count > 0)
                {
                    var document = analyzeResult.Documents[0];
                    foreach (var field in document.Fields)
                    {
                        string rawValue = field.Value.ValueString ?? field.Value.Content ?? field.Value.ToString();
                        camposRaw[field.Key] = rawValue;
                    }
                }

                // 🔧 NUEVO: Capturar TODOS los campos estructurados de Azure
                var camposEstructurados = ExtraerCamposEstructurados(analyzeResult);

                // Combinar campos raw + estructurados
                foreach (var campo in camposEstructurados)
                {
                    if (!camposRaw.ContainsKey(campo.Key))
                    {
                        camposRaw[campo.Key] = campo.Value;
                    }
                }

                _logger.LogInformation("📊 PASO 2: Procesando con SmartParser - Total campos: {Count}", camposRaw.Count);

                // ✅ NUEVO: Logear algunos campos clave antes del parsing
                LogCamposClave(camposRaw);

                // Usar tu SmartDocumentParser actualizado
                var smartData = _smartParser.ExtraerDatosInteligente(camposRaw);

                // 🔧 NUEVO: Post-procesamiento para completar campos faltantes
                CompletarCamposFaltantes(smartData, analyzeResult);

                // ✅ NUEVO: Post-procesamiento específico para pago
                CompletarDatosPago(smartData, camposRaw, analyzeResult);

                stopwatch.Stop();

                // Mapear a tu DTO existente - ✅ AHORA CON LAS NUEVAS PROPIEDADES
                var datosFormateados = new AzureDatosFormateadosDto
                {
                    NumeroPoliza = smartData.NumeroPoliza,
                    Asegurado = smartData.Asegurado,
                    Documento = smartData.Documento,
                    Vehiculo = smartData.Vehiculo,
                    Marca = smartData.Marca,
                    Modelo = smartData.Modelo,
                    Matricula = smartData.Matricula,
                    Motor = smartData.Motor,
                    Chasis = smartData.Chasis,
                    PrimaComercial = smartData.PrimaComercial,
                    PremioTotal = smartData.PremioTotal,
                    VigenciaDesde = smartData.VigenciaDesde,
                    VigenciaHasta = smartData.VigenciaHasta,
                    Corredor = smartData.Corredor,
                    Plan = smartData.Plan,
                    Ramo = smartData.Ramo,
                    Anio = smartData.Anio,
                    Email = smartData.Email,
                    Direccion = smartData.Direccion,
                    Departamento = smartData.Departamento,
                    Localidad = smartData.Localidad,
                    TipoVehiculo = smartData.TipoVehiculo,
                    Combustible = smartData.Combustible,
                    Uso = smartData.Uso,
                    ImpuestoMSP = smartData.ImpuestoMSP,
                    FormaPago = smartData.FormaPago,      
                    CantidadCuotas = smartData.CantidadCuotas, 
                    Telefono = smartData.Telefono,
                    CodigoPostal = smartData.CodigoPostal,
                    Descuentos = smartData.Descuentos,
                    Recargos = smartData.Recargos,
                    Color = smartData.Color                  
                };

                var response = new AzureProcessResponseDto
                {
                    Archivo = file.FileName,
                    Timestamp = DateTime.UtcNow,
                    TiempoProcesamiento = stopwatch.ElapsedMilliseconds,
                    Estado = "PROCESADO_CON_SMART_EXTRACTION",
                    DatosFormateados = datosFormateados,
                    SiguientePaso = "completar_formulario",
                    Resumen = new AzureResumenDto
                    {
                        ProcesamientoExitoso = true,
                        NumeroPolizaExtraido = datosFormateados.NumeroPoliza,
                        ClienteExtraido = datosFormateados.Asegurado,
                        DocumentoExtraido = datosFormateados.Documento,
                        VehiculoExtraido = datosFormateados.Vehiculo,
                        ListoParaVelneo = datosFormateados.TieneDatosMinimos
                    }
                };

                // ✅ NUEVO: Logear información de pago extraída
                _logger.LogInformation("💳 Pago extraído: FormaPago={FormaPago}, Cuotas={Cuotas}, Color={Color}",
                    datosFormateados.FormaPago, datosFormateados.CantidadCuotas, datosFormateados.Color);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en procesamiento principal");
                return StatusCode(500, AzureErrorResponseDto.ErrorGeneral(ex.Message, file?.FileName));
            }
        }

        // 🔧 NUEVO MÉTODO: Extraer campos estructurados adicionales de Azure
        private Dictionary<string, string> ExtraerCamposEstructurados(AnalyzeResult analyzeResult)
        {
            var campos = new Dictionary<string, string>();

            try
            {
                _logger.LogInformation("🔍 Extrayendo campos estructurados avanzados...");

                // 1. EXTRAER DE TODOS LOS DOCUMENTOS/FIELDS
                if (analyzeResult.Documents != null)
                {
                    foreach (var document in analyzeResult.Documents)
                    {
                        if (document.Fields != null)
                        {
                            foreach (var field in document.Fields)
                            {
                                var key = field.Key.ToLowerInvariant().Replace(" ", "_");
                                var value = field.Value.Content ?? field.Value.ValueString ?? "";

                                if (!string.IsNullOrEmpty(value))
                                {
                                    campos[key] = value;
                                    _logger.LogDebug("📄 Campo extraído: {Key} = {Value}", key, value.Substring(0, Math.Min(50, value.Length)));
                                }
                            }
                        }
                    }
                }

                // 2. EXTRAER DE KEY-VALUE PAIRS (MUY IMPORTANTE para tu tipo de documento)
                if (analyzeResult.KeyValuePairs != null)
                {
                    _logger.LogDebug("🔑 Procesando {Count} key-value pairs", analyzeResult.KeyValuePairs.Count);

                    foreach (var kvp in analyzeResult.KeyValuePairs)
                    {
                        var key = kvp.Key?.Content?.ToLowerInvariant()
                            .Replace(" ", "_")
                            .Replace(":", "")
                            .Replace(".", "")
                            .Replace("-", "_") ?? "";

                        var value = kvp.Value?.Content?.Trim() ?? "";

                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && value.Length > 1)
                        {
                            // Mapear keys comunes a nombres estándar
                            key = MapearKeyComun(key);
                            campos[key] = value;
                            _logger.LogDebug("🔑 KVP extraído: {Key} = {Value}", key, value);
                        }
                    }
                }

                // 3. EXTRAER DE TABLAS ESTRUCTURADAS (CRÍTICO para información de pagos)
                if (analyzeResult.Tables != null)
                {
                    _logger.LogDebug("📊 Procesando {Count} tablas", analyzeResult.Tables.Count);

                    foreach (var table in analyzeResult.Tables)
                    {
                        ExtraerDatosDeTabla(table, campos);
                    }
                }

                // 4. EXTRAER DEL CONTENIDO COMPLETO con regex mejorados
                if (!string.IsNullOrEmpty(analyzeResult.Content))
                {
                    ExtraerConRegexAvanzados(analyzeResult.Content, campos);
                }

                _logger.LogInformation("✅ Extraídos {Count} campos estructurados", campos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo campos estructurados");
            }

            return campos;
        }

        private string MapearKeyComun(string key)
        {
            var mappings = new Dictionary<string, string>
            {
                {"matricula", "matricula"},
                {"matrícula", "matricula"},
                {"padron", "matricula"},
                {"padrón", "matricula"},
                {"nro_padron", "matricula"},
                {"direccion", "direccion"},
                {"dirección", "direccion"},
                {"domicilio", "direccion"},
                {"telefono", "telefono"},
                {"teléfono", "telefono"},
                {"tel", "telefono"},
                {"celular", "telefono"},
                {"email", "email"},
                {"e_mail", "email"},
                {"correo", "email"},
                {"mail", "email"},
                {"forma_pago", "forma_pago"},
                {"forma_de_pago", "forma_pago"},
                {"cuotas", "cuotas"},
                {"cant_cuotas", "cuotas"},
                {"cantidad_cuotas", "cuotas"},
                {"color", "color"},
                {"tipo", "tipo_vehiculo"},
                {"tipo_vehiculo", "tipo_vehiculo"},
                {"uso", "uso"},
                {"combustible", "combustible"},
                {"codigo_postal", "codigo_postal"},
                {"cp", "codigo_postal"}
            };

            return mappings.ContainsKey(key) ? mappings[key] : key;
        }

        private void ExtraerDatosDeTabla(DocumentTable table, Dictionary<string, string> campos)
        {
            try
            {
                if (table.Cells == null) return;

                // Organizar celdas por filas y columnas
                var filas = new Dictionary<int, Dictionary<int, string>>();

                foreach (var cell in table.Cells)
                {
                    if (!filas.ContainsKey(cell.RowIndex))
                        filas[cell.RowIndex] = new Dictionary<int, string>();

                    filas[cell.RowIndex][cell.ColumnIndex] = cell.Content ?? "";
                }

                // Buscar patrones específicos en las tablas
                foreach (var fila in filas)
                {
                    var celdas = fila.Value.Values.ToList();
                    var textoFila = string.Join(" ", celdas).ToLowerInvariant();

                    // PATRÓN 1: Buscar información de vehículo en tablas
                    if (textoFila.Contains("matrícula") || textoFila.Contains("matricula") || textoFila.Contains("padrón"))
                    {
                        foreach (var celda in celdas)
                        {
                            var matricula = ExtraerMatriculaDeTexto(celda);
                            if (!string.IsNullOrEmpty(matricula))
                            {
                                campos["matricula"] = matricula;
                                _logger.LogDebug("🚗 Matrícula encontrada en tabla: {Matricula}", matricula);
                            }
                        }
                    }

                    // PATRÓN 2: Buscar información de contacto
                    if (textoFila.Contains("dirección") || textoFila.Contains("direccion") || textoFila.Contains("domicilio"))
                    {
                        foreach (var celda in celdas)
                        {
                            if (EsDireccionValida(celda))
                            {
                                campos["direccion"] = celda;
                                _logger.LogDebug("🏠 Dirección encontrada en tabla: {Direccion}", celda);
                            }
                        }
                    }

                    // PATRÓN 3: Buscar teléfonos
                    if (textoFila.Contains("teléfono") || textoFila.Contains("telefono") || textoFila.Contains("tel"))
                    {
                        foreach (var celda in celdas)
                        {
                            var telefono = ExtraerTelefonoDeTexto(celda);
                            if (!string.IsNullOrEmpty(telefono))
                            {
                                campos["telefono"] = telefono;
                                _logger.LogDebug("📞 Teléfono encontrado en tabla: {Telefono}", telefono);
                            }
                        }
                    }

                    // PATRÓN 4: Buscar forma de pago y cuotas
                    if (textoFila.Contains("pago") || textoFila.Contains("cuotas") || textoFila.Contains("mensual"))
                    {
                        foreach (var celda in celdas)
                        {
                            if (EsFormaPago(celda))
                            {
                                campos["forma_pago"] = celda;
                                _logger.LogDebug("💳 Forma de pago encontrada: {FormaPago}", celda);
                            }

                            var cuotas = ExtraerNumeroCuotas(celda);
                            if (cuotas > 0)
                            {
                                campos["cuotas"] = cuotas.ToString();
                                _logger.LogDebug("📊 Cuotas encontradas: {Cuotas}", cuotas);
                            }
                        }
                    }

                    // PATRÓN 5: Buscar información adicional del vehículo
                    if (textoFila.Contains("color") || textoFila.Contains("tipo") || textoFila.Contains("uso") || textoFila.Contains("combustible"))
                    {
                        ExtraerDatosVehiculoDeTabla(celdas, campos);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo datos de tabla");
            }
        }

        private void ExtraerConRegexAvanzados(string content, Dictionary<string, string> campos)
        {
            try
            {
                // MATRÍCULA - Patrones uruguayos específicos
                var matriculaPatterns = new[]
                {
            @"(?i)matrícula[:\s]*([A-Z]{3}\s*\d{4})",           // ABC 1234
            @"(?i)matricula[:\s]*([A-Z]{3}\s*\d{4})",           // ABC 1234
            @"(?i)padrón[:\s]*([A-Z]{3}\s*\d{4})",              // ABC 1234
            @"(?i)padron[:\s]*([A-Z]{3}\s*\d{4})",              // ABC 1234
            @"([A-Z]{3}\s*\d{4})(?=\s|$|\n)",                   // ABC 1234 standalone
            @"(?i)registro[:\s]*([A-Z]{3}\s*\d{4})"             // Registro: ABC 1234
        };

                foreach (var pattern in matriculaPatterns)
                {
                    var match = Regex.Match(content, pattern);
                    if (match.Success && !campos.ContainsKey("matricula"))
                    {
                        var matricula = match.Groups[1].Value.Trim().Replace(" ", "");
                        if (matricula.Length >= 6) // Validación básica
                        {
                            campos["matricula"] = matricula;
                            _logger.LogDebug("🚗 Matrícula encontrada con regex: {Matricula}", matricula);
                            break;
                        }
                    }
                }

                // DIRECCIÓN - Patrones uruguayos
                var direccionPatterns = new[]
                {
            @"(?i)dirección[:\s]*([^.]{10,100}?)(?:\s+localidad|\s+depto|\s+montevideo|$|\n)",
            @"(?i)direccion[:\s]*([^.]{10,100}?)(?:\s+localidad|\s+depto|\s+montevideo|$|\n)",
            @"(?i)domicilio[:\s]*([^.]{10,100}?)(?:\s+localidad|\s+depto|\s+montevideo|$|\n)"
        };

                foreach (var pattern in direccionPatterns)
                {
                    var match = Regex.Match(content, pattern);
                    if (match.Success && !campos.ContainsKey("direccion"))
                    {
                        var direccion = match.Groups[1].Value.Trim();
                        if (EsDireccionValida(direccion))
                        {
                            campos["direccion"] = direccion;
                            _logger.LogDebug("🏠 Dirección encontrada con regex: {Direccion}", direccion);
                            break;
                        }
                    }
                }

                // TELÉFONO - Patrones uruguayos
                var telefonoPatterns = new[]
                {
            @"(?i)teléfono[:\s]*(\+?598\s*\d{8})",              // +598 12345678
            @"(?i)telefono[:\s]*(\+?598\s*\d{8})",              // +598 12345678
            @"(?i)tel[:\s]*(\+?598\s*\d{8})",                   // Tel: +598 12345678
            @"(?i)celular[:\s]*(\+?598\s*9\d{7})",              // Celular uruguayo
            @"(\+?598\s*\d{8,9})",                               // Standalone
            @"(?i)contacto[:\s]*(\+?598\s*\d{8})"               // Contacto: +598
        };

                foreach (var pattern in telefonoPatterns)
                {
                    var match = Regex.Match(content, pattern);
                    if (match.Success && !campos.ContainsKey("telefono"))
                    {
                        var telefono = match.Groups[1].Value.Trim();
                        campos["telefono"] = telefono;
                        _logger.LogDebug("📞 Teléfono encontrado con regex: {Telefono}", telefono);
                        break;
                    }
                }

                // FORMA DE PAGO
                var formaPagoPatterns = new[]
                {
            @"(?i)forma\s+de\s+pago[:\s]*([^.]{3,30}?)(?:\s|$|\n)",
            @"(?i)modalidad[:\s]*([^.]{3,30}?)(?:\s|$|\n)",
            @"(?i)pago[:\s]*(mensual|anual|semestral|trimestral|contado)",
            @"(?i)(mensual|anual|semestral|trimestral|contado)(?=\s|$|\n)"
        };

                foreach (var pattern in formaPagoPatterns)
                {
                    var match = Regex.Match(content, pattern);
                    if (match.Success && !campos.ContainsKey("forma_pago"))
                    {
                        var formaPago = match.Groups[1].Value.Trim();
                        if (EsFormaPago(formaPago))
                        {
                            campos["forma_pago"] = formaPago;
                            _logger.LogDebug("💳 Forma de pago encontrada: {FormaPago}", formaPago);
                            break;
                        }
                    }
                }

                _logger.LogDebug("✅ Regex avanzados completados");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error en regex avanzados");
            }
        }

        private string ExtraerMatriculaDeTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";

            // Patrón uruguayo: ABC1234 o ABC 1234
            var match = Regex.Match(texto, @"([A-Z]{3}\s*\d{4})", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Replace(" ", "") : "";
        }

        private bool EsDireccionValida(string texto)
        {
            if (string.IsNullOrEmpty(texto) || texto.Length < 10) return false;

            // Debe contener números (típico en direcciones)
            if (!Regex.IsMatch(texto, @"\d")) return false;

            // No debe ser solo números
            if (Regex.IsMatch(texto, @"^\d+$")) return false;

            return true;
        }

        private string ExtraerTelefonoDeTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";

            var patterns = new[]
            {
                @"(\+?598\s*\d{8,9})",
                @"(\d{8,9})"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(texto, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            return "";
        }

        private bool EsFormaPago(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return false;

            var formasPago = new[] { "mensual", "anual", "semestral", "trimestral", "contado", "12", "6", "3", "1" };
            return formasPago.Any(forma => texto.ToLowerInvariant().Contains(forma));
        }

        private int ExtraerNumeroCuotas(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return 0;

            var match = Regex.Match(texto, @"(\d{1,2})\s*cuotas?", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
            {
                return cuotas;
            }

            return 0;
        }

        private void ExtraerDatosVehiculoDeTabla(List<string> celdas, Dictionary<string, string> campos)
        {
            foreach (var celda in celdas)
            {
                var texto = celda.ToLowerInvariant();

                if (texto.Contains("color") && !campos.ContainsKey("color"))
                {
                    var colores = new[] { "blanco", "negro", "gris", "plata", "azul", "rojo", "verde", "amarillo" };
                    foreach (var color in colores)
                    {
                        if (texto.Contains(color))
                        {
                            campos["color"] = color;
                            break;
                        }
                    }
                }

                if (texto.Contains("combustible") && !campos.ContainsKey("combustible"))
                {
                    var combustibles = new[] { "gasolina", "diesel", "gas", "eléctrico", "híbrido" };
                    foreach (var combustible in combustibles)
                    {
                        if (texto.Contains(combustible))
                        {
                            campos["combustible"] = combustible;
                            break;
                        }
                    }
                }

                if (texto.Contains("uso") && !campos.ContainsKey("uso"))
                {
                    var usos = new[] { "particular", "comercial", "taxi", "remise", "carga" };
                    foreach (var uso in usos)
                    {
                        if (texto.Contains(uso))
                        {
                            campos["uso"] = uso;
                            break;
                        }
                    }
                }
            }
        }

        // 🔧 NUEVO MÉTODO: Post-procesamiento para completar campos que faltan
        private void CompletarCamposFaltantes(SmartExtractedData datos, AnalyzeResult analyzeResult)
        {
            try
            {
                // Si campos críticos están vacíos, buscar con patterns más agresivos
                if (string.IsNullOrEmpty(datos.Marca))
                {
                    datos.Marca = BuscarEnTodoElTexto(analyzeResult, @"(?i)(TOYOTA|RENAULT|CHEVROLET|FORD|VOLKSWAGEN|PEUGEOT|FIAT|NISSAN|HYUNDAI|KIA|HONDA|MAZDA)");
                }

                if (string.IsNullOrEmpty(datos.Modelo) && !string.IsNullOrEmpty(datos.Marca))
                {
                    datos.Modelo = BuscarEnTodoElTexto(analyzeResult, $@"(?i){datos.Marca}\s+([A-Za-z0-9\s]+?)(?:\s+\d{{4}}|\s+año|\n|$)");
                }

                if (string.IsNullOrEmpty(datos.Motor))
                {
                    datos.Motor = BuscarEnTodoElTexto(analyzeResult, @"(?i)motor[:\s]*([A-Z0-9]{6,})") ??
                                 BuscarEnTodoElTexto(analyzeResult, @"([A-Z]{2}\d{6,})");
                }

                if (string.IsNullOrEmpty(datos.Email))
                {
                    datos.Email = BuscarEnTodoElTexto(analyzeResult, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
                }

                // Completar descripción del vehículo si está incompleta
                if (string.IsNullOrEmpty(datos.Vehiculo) || datos.Vehiculo.Trim() == "()")
                {
                    var partes = new List<string>();
                    if (!string.IsNullOrEmpty(datos.Marca)) partes.Add(datos.Marca);
                    if (!string.IsNullOrEmpty(datos.Modelo)) partes.Add(datos.Modelo);
                    if (!string.IsNullOrEmpty(datos.Anio)) partes.Add($"({datos.Anio})");

                    if (partes.Any())
                    {
                        datos.Vehiculo = string.Join(" ", partes);
                    }
                }

                _logger.LogDebug("✅ Post-procesamiento completado");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error en post-procesamiento");
            }
        }

        // 🔧 MÉTODO AUXILIAR: Buscar en todo el contenido del documento
        private string BuscarEnTodoElTexto(AnalyzeResult analyzeResult, string pattern)
        {
            try
            {
                var textoCompleto = "";

                // Concatenar todo el texto del documento
                if (analyzeResult.Content != null)
                {
                    textoCompleto = analyzeResult.Content;
                }

                var match = System.Text.RegularExpressions.Regex.Match(textoCompleto, pattern, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error buscando pattern en texto completo");
            }

            return "";
        }

        [HttpPost("process-batch")]
        [Authorize]
        [ProducesResponseType(typeof(AzureBatchResponseDto), 200)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 400)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 500)]
        public async Task<ActionResult> ProcessBatch([Required] List<IFormFile> files)
        {
            var stopwatchTotal = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(AzureErrorResponseDto.ArchivoInvalido("Lista de archivos vacía"));
                }

                _logger.LogInformation("📦 PROCESAMIENTO EN LOTE: {FileCount} archivos", files.Count);

                var resultados = new List<AzureProcessResponseDto>();
                var errores = new List<AzureBatchErrorDto>();

                foreach (var file in files)
                {
                    try
                    {
                        var singleFileResult = await ProcessSingleFileInternal(file);
                        resultados.Add(singleFileResult);
                    }
                    catch (Exception ex)
                    {
                        errores.Add(new AzureBatchErrorDto
                        {
                            Archivo = file.FileName,
                            Error = ex.Message,
                            Timestamp = DateTime.UtcNow,
                            CodigoError = "ERROR_PROCESAMIENTO_INDIVIDUAL"
                        });
                    }
                }

                stopwatchTotal.Stop();

                var response = new AzureBatchResponseDto
                {
                    Procesados = resultados.Count,
                    Errores = errores.Count,
                    TotalArchivos = files.Count,
                    Resultados = resultados,
                    ErroresDetalle = errores,
                    FechaProcesamiento = DateTime.UtcNow,
                    TiempoTotalProcesamiento = stopwatchTotal.ElapsedMilliseconds
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en procesamiento en lote");
                return StatusCode(500, AzureErrorResponseDto.ErrorGeneral(ex.Message, "Procesamiento en lote"));
            }
        }

        [HttpGet("model-info")]
        [Authorize]
        [ProducesResponseType(typeof(AzureModelInfoResponseDto), 200)]
        [ProducesResponseType(typeof(AzureErrorResponseDto), 500)]
        public async Task<ActionResult> GetModelInfo()
        {
            try
            {
                var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
                var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                var endpointsToTry = new[]
                {
                    $"{endpoint.TrimEnd('/')}/documentintelligence/documentModels/{modelId}?api-version=2024-02-29-preview",
                    $"{endpoint.TrimEnd('/')}/documentintelligence/documentModels/{modelId}?api-version=2023-07-31",
                    $"{endpoint.TrimEnd('/')}/formrecognizer/documentModels/{modelId}?api-version=2022-08-31"
                };

                foreach (var url in endpointsToTry)
                {
                    try
                    {
                        _logger.LogInformation("Probando endpoint: {Url}", url);
                        var response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            _logger.LogInformation("✅ ÉXITO con endpoint: {Url}", url);

                            var modelInfoResponse = new AzureModelInfoResponseDto
                            {
                                ModelId = modelId,
                                Endpoint = endpoint,
                                Status = "Modelo encontrado y activo",
                                WorkingApiUrl = url,
                                HttpStatus = response.StatusCode.ToString(),
                                ApiVersion = url.Contains("2024-02-29") ? "2024-02-29-preview" :
                                           url.Contains("2023-07-31") ? "2023-07-31" : "2022-08-31",
                                ConsultaTimestamp = DateTime.UtcNow
                            };

                            return Ok(modelInfoResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error con endpoint {Url}: {Error}", url, ex.Message);
                        continue;
                    }
                }

                var warningResponse = new AzureModelInfoResponseDto
                {
                    ModelId = modelId,
                    Endpoint = endpoint,
                    Status = "Configurado pero no accesible",
                    Warning = "El modelo está configurado pero no se pudo acceder a información detallada",
                    Message = "Esto es normal si el modelo es personalizado o está en una región específica",
                    ConsultaTimestamp = DateTime.UtcNow
                };

                return Ok(warningResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo información del modelo");
                return StatusCode(500, AzureErrorResponseDto.ErrorGeneral(ex.Message, "Consulta de modelo"));
            }
        }

        #region Métodos de Debugging y Post-procesamiento

        private void LogCamposClave(Dictionary<string, string> campos)
        {
            _logger.LogDebug("🔍 CAMPOS CLAVE ENCONTRADOS:");

            var camposInteres = new[] {
        "medio_pago", "forma_pago", "cuotas", "pago", "payment",
        "mensual", "anual", "modo_facturacion", "installments"
    };

            foreach (var campo in campos)
            {
                foreach (var interes in camposInteres)
                {
                    if (campo.Key.Contains(interes, StringComparison.OrdinalIgnoreCase) ||
                        campo.Value.Contains(interes, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogDebug("💰 Campo pago: {Key} = {Value}",
                            campo.Key, campo.Value.Substring(0, Math.Min(100, campo.Value.Length)));
                        break;
                    }
                }
            }

            // Logear campos que contienen números (posibles cuotas)
            foreach (var campo in campos)
            {
                if (Regex.IsMatch(campo.Value, @"\d+\s*cuotas?", RegexOptions.IgnoreCase))
                {
                    _logger.LogDebug("🔢 Campo con cuotas: {Key} = {Value}",
                        campo.Key, campo.Value.Substring(0, Math.Min(100, campo.Value.Length)));
                }
            }
        }

        private void CompletarDatosPago(SmartExtractedData datos, Dictionary<string, string> camposRaw, AnalyzeResult analyzeResult)
        {
            try
            {
                _logger.LogDebug("💳 Post-procesando datos de pago...");

                // Si no se encontró forma de pago, buscar con patterns más agresivos
                if (string.IsNullOrEmpty(datos.FormaPago))
                {
                    var formaPago = BuscarFormaPagoEnTodoElTexto(analyzeResult.Content);
                    if (!string.IsNullOrEmpty(formaPago))
                    {
                        datos.FormaPago = formaPago;
                        _logger.LogDebug("✅ Forma de pago encontrada en texto completo: {FormaPago}", formaPago);
                    }
                }

                // Si no se encontraron cuotas específicas, buscar patterns especiales
                if (datos.CantidadCuotas <= 1)
                {
                    var cuotas = BuscarCuotasEnTodoElTexto(analyzeResult.Content);
                    if (cuotas > 1)
                    {
                        datos.CantidadCuotas = cuotas;
                        _logger.LogDebug("✅ Cuotas encontradas en texto completo: {Cuotas}", cuotas);
                    }
                }

                // Buscar en campos específicos que puedan tener información de pago
                BuscarEnCamposEspecificosPago(camposRaw, datos);

                // Última verificación de relaciones lógicas
                AjustarRelacionesFinales(datos);

                _logger.LogDebug("💳 Resultado final: FormaPago='{FormaPago}', Cuotas={Cuotas}",
                    datos.FormaPago, datos.CantidadCuotas);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error en post-procesamiento de datos de pago");
            }
        }

        private string BuscarFormaPagoEnTodoElTexto(string textoCompleto)
        {
            if (string.IsNullOrEmpty(textoCompleto)) return "";

            var patterns = new[]
            {
        @"(?i)MEDIO\s+DE\s+PAGO[:\s]*([^\n\r.]{5,50})",
        @"(?i)FORMA\s+DE\s+PAGO[:\s]*([^\n\r.]{5,50})",
        @"(?i)MODALIDAD[:\s]*([^\n\r.]{5,50})",
        @"(?i)(MENSUAL|ANUAL|SEMESTRAL|TRIMESTRAL|CONTADO)(?=\s|$|\n|\r)",
        @"(\d{1,2})\s*CUOTAS?\s*(MENSUAL|ANUAL|SEMESTRAL)?",
        @"PAGO\s+(MENSUAL|ANUAL|SEMESTRAL|TRIMESTRAL|CONTADO)"
    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(textoCompleto, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var valor = match.Groups[1].Value.Trim().ToUpperInvariant();

                    // Mapear valores comunes
                    if (valor.Contains("MENSUAL") || valor == "12")
                        return "MENSUAL";
                    else if (valor.Contains("ANUAL") || valor == "1")
                        return "ANUAL";
                    else if (valor.Contains("SEMESTRAL") || valor == "6")
                        return "SEMESTRAL";
                    else if (valor.Contains("TRIMESTRAL") || valor == "3")
                        return "TRIMESTRAL";
                    else if (valor.Contains("CONTADO"))
                        return "CONTADO";
                    else if (!string.IsNullOrEmpty(valor) && valor.Length > 2)
                        return valor;
                }
            }

            return "";
        }

        private int BuscarCuotasEnTodoElTexto(string textoCompleto)
        {
            if (string.IsNullOrEmpty(textoCompleto)) return 0;

            var patterns = new[]
            {
        @"(\d{1,2})\s*CUOTAS?",
        @"CUOTAS?\s*[:\-]\s*(\d{1,2})",
        @"PAGO\s+EN\s+(\d{1,2})",
        @"(\d{1,2})\s*MENSUAL",
        @"MENSUAL\s*(\d{1,2})"
    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(textoCompleto, pattern, RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
                {
                    if (cuotas >= 1 && cuotas <= 48) // Validar rango razonable
                        return cuotas;
                }
            }

            return 0;
        }

        private void BuscarEnCamposEspecificosPago(Dictionary<string, string> campos, SmartExtractedData datos)
        {
            // Buscar patrones específicos según tu gist
            foreach (var kvp in campos)
            {
                var texto = kvp.Value;

                // Buscar "12 CUOTAS" específicamente
                if (datos.CantidadCuotas <= 1 && texto.Contains("CUOTAS", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(texto, @"(\d{1,2})\s*CUOTAS?", RegexOptions.IgnoreCase);
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
                    {
                        datos.CantidadCuotas = cuotas;
                        _logger.LogDebug("✅ Cuotas específicas encontradas: {Cuotas}", cuotas);

                        // Si encontramos cuotas pero no forma de pago, inferir
                        if (string.IsNullOrEmpty(datos.FormaPago))
                        {
                            datos.FormaPago = cuotas switch
                            {
                                1 => "CONTADO",
                                3 => "TRIMESTRAL",
                                6 => "SEMESTRAL",
                                12 => "MENSUAL",
                                _ => "MENSUAL"
                            };
                        }
                    }
                }

                // Buscar "MEDIO DE PAGO" específicamente
                if (string.IsNullOrEmpty(datos.FormaPago) &&
                    (kvp.Key.Contains("medio", StringComparison.OrdinalIgnoreCase) ||
                     texto.Contains("MEDIO DE PAGO", StringComparison.OrdinalIgnoreCase)))
                {
                    var patterns = new[]
                    {
                @"MEDIO\s+DE\s+PAGO[:\s]*([^\n\r]{3,50})",
                @"medio_pago[:\s]*([^\n\r]{3,50})",
                @"payment_method[:\s]*([^\n\r]{3,50})"
            };

                    foreach (var pattern in patterns)
                    {
                        var match = Regex.Match(texto, pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            var medioPago = match.Groups[1].Value.Trim();
                            if (!string.IsNullOrEmpty(medioPago) && medioPago.Length > 2)
                            {
                                datos.FormaPago = medioPago.ToUpperInvariant();
                                _logger.LogDebug("✅ Medio de pago específico encontrado: {MedioPago}", medioPago);
                                break;
                            }
                        }
                    }
                }

                // Buscar campos que contengan información combinada de pago
                if (texto.Contains("CUOTAS", StringComparison.OrdinalIgnoreCase) &&
                    (texto.Contains("MENSUAL", StringComparison.OrdinalIgnoreCase) ||
                     texto.Contains("ANUAL", StringComparison.OrdinalIgnoreCase)))
                {
                    // Ejemplo: "12 CUOTAS MENSUALES"
                    var matchCombinado = Regex.Match(texto, @"(\d{1,2})\s*CUOTAS?\s*(MENSUAL|ANUAL|SEMESTRAL|TRIMESTRAL)?", RegexOptions.IgnoreCase);
                    if (matchCombinado.Success)
                    {
                        if (int.TryParse(matchCombinado.Groups[1].Value, out int cuotas))
                        {
                            datos.CantidadCuotas = cuotas;
                        }

                        var tipoPago = matchCombinado.Groups[2].Value;
                        if (!string.IsNullOrEmpty(tipoPago))
                        {
                            datos.FormaPago = tipoPago.ToUpperInvariant();
                        }

                        _logger.LogDebug("✅ Información combinada encontrada: {Cuotas} {FormaPago}", datos.CantidadCuotas, datos.FormaPago);
                    }
                }
            }
        }

        private void AjustarRelacionesFinales(SmartExtractedData datos)
        {
            // Validaciones finales y ajustes de coherencia

            // Si tenemos forma de pago pero cuotas incorrectas
            if (!string.IsNullOrEmpty(datos.FormaPago))
            {
                switch (datos.FormaPago.ToUpperInvariant())
                {
                    case "MENSUAL":
                        if (datos.CantidadCuotas <= 1 || datos.CantidadCuotas > 24)
                            datos.CantidadCuotas = 12; // Default mensual
                        break;
                    case "ANUAL":
                    case "CONTADO":
                        datos.CantidadCuotas = 1;
                        break;
                    case "SEMESTRAL":
                        if (datos.CantidadCuotas <= 1 || datos.CantidadCuotas > 12)
                            datos.CantidadCuotas = 6;
                        break;
                    case "TRIMESTRAL":
                        if (datos.CantidadCuotas <= 1 || datos.CantidadCuotas > 8)
                            datos.CantidadCuotas = 3;
                        break;
                }
            }

            // Si tenemos cuotas válidas pero no forma de pago
            if (string.IsNullOrEmpty(datos.FormaPago) && datos.CantidadCuotas > 1)
            {
                datos.FormaPago = datos.CantidadCuotas switch
                {
                    1 => "CONTADO",
                    2 => "SEMESTRAL",
                    3 => "TRIMESTRAL",
                    4 => "TRIMESTRAL",
                    6 => "SEMESTRAL",
                    12 => "MENSUAL",
                    _ when datos.CantidadCuotas > 6 => "MENSUAL",
                    _ => "MENSUAL"
                };
            }

            // Default si no se encontró nada
            if (string.IsNullOrEmpty(datos.FormaPago) && datos.CantidadCuotas <= 1)
            {
                datos.FormaPago = "CONTADO";
                datos.CantidadCuotas = 1;
            }
        }

        #endregion

        #region Métodos Privados de Soporte

        private async Task<AzureProcessResponseDto> ProcessSingleFileInternal(IFormFile file)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
            var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];
            var modelId = _configuration["AzureDocumentIntelligence:ModelId"];

            var client = new DocumentIntelligenceClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey));

            using var stream = file.OpenReadStream();
            var binaryData = BinaryData.FromStream(stream);

            var operation = await client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                modelId,
                binaryData);

            var analyzeResult = operation.Value;
            var camposRaw = new Dictionary<string, string>();

            if (analyzeResult.Documents?.Count > 0)
            {
                var document = analyzeResult.Documents[0];
                foreach (var field in document.Fields)
                {
                    string rawValue = field.Value.ValueString ?? field.Value.Content ?? "";
                    camposRaw[field.Key] = rawValue.Trim();
                }
            }

            var datosFormateados = _smartParser.ExtraerDatosInteligente(camposRaw);

            stopwatch.Stop();

            // ✅ RESPUESTA SIMPLIFICADA PARA PROCESAMIENTO EN LOTE
            return new AzureProcessResponseDto
            {
                Archivo = file.FileName,
                Timestamp = DateTime.UtcNow,
                TiempoProcesamiento = stopwatch.ElapsedMilliseconds,
                Estado = "PROCESADO_CON_SMART_EXTRACTION",
                DatosFormateados = new AzureDatosFormateadosDto
                {
                    NumeroPoliza = datosFormateados.NumeroPoliza,
                    Asegurado = datosFormateados.Asegurado,
                    Documento = datosFormateados.Documento,
                    Vehiculo = datosFormateados.Vehiculo,
                    Marca = datosFormateados.Marca,
                    Modelo = datosFormateados.Modelo,
                    Matricula = datosFormateados.Matricula,
                    Motor = datosFormateados.Motor,
                    Chasis = datosFormateados.Chasis,
                    PrimaComercial = datosFormateados.PrimaComercial,
                    PremioTotal = datosFormateados.PremioTotal,
                    VigenciaDesde = datosFormateados.VigenciaDesde,
                    VigenciaHasta = datosFormateados.VigenciaHasta,
                    Corredor = datosFormateados.Corredor,
                    Plan = datosFormateados.Plan,
                    Ramo = datosFormateados.Ramo,
                    Anio = datosFormateados.Anio,
                    Email = datosFormateados.Email,
                    Direccion = datosFormateados.Direccion,
                    Departamento = datosFormateados.Departamento,
                    Localidad = datosFormateados.Localidad
                },
                // ✅ SIN BÚSQUEDA DE CLIENTE EN PROCESAMIENTO BATCH
                // BusquedaCliente = null,
                SiguientePaso = "completar_formulario",
                Resumen = new AzureResumenDto
                {
                    ProcesamientoExitoso = true,
                    NumeroPolizaExtraido = datosFormateados.NumeroPoliza,
                    ClienteExtraido = datosFormateados.Asegurado,
                    DocumentoExtraido = datosFormateados.Documento,
                    VehiculoExtraido = datosFormateados.Vehiculo,
                    ClienteEncontrado = false,
                    ListoParaVelneo = !string.IsNullOrEmpty(datosFormateados.NumeroPoliza) &&
                                     !string.IsNullOrEmpty(datosFormateados.Asegurado)
                }
            };
        }

        #endregion
    }
}