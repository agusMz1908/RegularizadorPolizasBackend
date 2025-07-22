using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Azure;
using System.Text.RegularExpressions;

namespace RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence
{
    public class SmartDocumentParser
    {
        private readonly ILogger<SmartDocumentParser> _logger;

        public SmartDocumentParser(ILogger<SmartDocumentParser> logger)
        {
            _logger = logger;
        }

        public SmartExtractedData ExtraerDatosInteligente(Dictionary<string, string> camposExtraidos)
        {
            var datos = new SmartExtractedData();

            try
            {
                _logger.LogDebug("🧠 Iniciando extracción inteligente de {CamposCount} campos", camposExtraidos.Count);

                // 1. DATOS DE LA PÓLIZA
                if (camposExtraidos.ContainsKey("datos_poliza"))
                {
                    ExtractPolizaData(camposExtraidos["datos_poliza"], datos);
                }

                // 2. DATOS DEL ASEGURADO
                if (camposExtraidos.ContainsKey("datos_asegurado"))
                {
                    ExtractAseguradoData(camposExtraidos["datos_asegurado"], datos);
                }

                // 3. DATOS DEL VEHÍCULO
                if (camposExtraidos.ContainsKey("datos_vehiculo"))
                {
                    ExtractVehiculoData(camposExtraidos["datos_vehiculo"], datos);
                }

                // 4. DATOS DEL CORREDOR
                if (camposExtraidos.ContainsKey("datos_corredor"))
                {
                    ExtractCorredorData(camposExtraidos["datos_corredor"], datos);
                }

                // 5. PLAN
                if (camposExtraidos.ContainsKey("poliza.plan"))
                {
                    datos.Plan = ExtraerValorConRegex(camposExtraidos["poliza.plan"], @"Plan:\s*(.+)", "").Trim();
                }

                // 6. EMAIL
                if (camposExtraidos.ContainsKey("datos_envio"))
                {
                    datos.Email = ExtraerValorConRegex(camposExtraidos["datos_envio"], @"email:\s*([^\s]+@[^\s]+)", "");
                }

                // ✅ NUEVO 7. DATOS DE PAGO Y CUOTAS
                ExtractDatosPago(camposExtraidos, datos);

                // ✅ NUEVO 8. DATOS ADICIONALES
                ExtractDatosAdicionales(camposExtraidos, datos);

                // Construir descripción completa del vehículo
                if (string.IsNullOrEmpty(datos.Vehiculo))
                {
                    datos.Vehiculo = $"{datos.Marca} {datos.Modelo} ({datos.Anio})".Trim();
                }

                _logger.LogInformation("✅ Extracción inteligente completada: Póliza={Poliza}, Cliente={Cliente}, FormaPago={FormaPago}, Cuotas={Cuotas}",
                    datos.NumeroPoliza, datos.Asegurado, datos.FormaPago, datos.CantidadCuotas);

                return datos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en extracción inteligente");
                return datos; // Retorna datos parciales
            }
        }


        #region Métodos de Extracción Específicos

        private void ExtractPolizaData(string datosPoliza, SmartExtractedData datos)
        {
            // Número de póliza: "Nº de Póliza / Endoso: 9128263 / 0"
            datos.NumeroPoliza = ExtraerValorConRegex(datosPoliza, @"Nº de Póliza[^:]*:\s*(\d+)", "");

            // Prima comercial: "Prima Comercial: $ 123.584,47"
            var primaComercialStr = ExtraerValorConRegex(datosPoliza, @"Prima Comercial:\s*\$?\s*([\d,.]+)", "0");
            datos.PrimaComercial = ParsearNumero(primaComercialStr);

            // Premio total: "PREMIO TOTAL A PAGAR: $ 153.790,00"
            var premioTotalStr = ExtraerValorConRegex(datosPoliza, @"PREMIO TOTAL A PAGAR:\s*\$?\s*([\d,.]+)", "0");
            datos.PremioTotal = ParsearNumero(premioTotalStr);

            // Vigencias: "Vigencia: 22/03/2024 - 21/03/2027"
            var vigenciaMatch = Regex.Match(datosPoliza, @"Vigencia:\s*(\d{2}/\d{2}/\d{4})\s*-\s*(\d{2}/\d{2}/\d{4})");
            if (vigenciaMatch.Success)
            {
                if (DateTime.TryParseExact(vigenciaMatch.Groups[1].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var desde))
                    datos.VigenciaDesde = desde;

                if (DateTime.TryParseExact(vigenciaMatch.Groups[2].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var hasta))
                    datos.VigenciaHasta = hasta;
            }
        }

        private void ExtractAseguradoData(string datosAsegurado, SmartExtractedData datos)
        {
            // PROBLEMA: El regex actual no captura bien "RAYDORAT SA"
            // ANTERIOR: @"Asegurado:\s*([^R]+?)(?:\s+Radio:|$)"
            // NUEVO: Más flexible para capturar hasta "Radio:"
            datos.Asegurado = ExtraerValorConRegex(datosAsegurado, @"Asegurado:\s*([^R]+?)(?:\s+Radio:|\s+R\w+:|$)", "").Trim();

            // Si no funciona, probar alternativa más simple
            if (string.IsNullOrEmpty(datos.Asegurado))
            {
                datos.Asegurado = ExtraerValorConRegex(datosAsegurado, @"Asegurado:\s*(.+?)(?:\s+Radio|$)", "").Trim();
            }

            datos.Documento = ExtraerValorConRegex(datosAsegurado, @"RUT\s*-\s*(\d+)", "");

            // PROBLEMA: Dirección también está vacía
            // ANTERIOR: @"Dirección:\s*([^L]+?)(?:\s+Localidad:|$)"
            // NUEVO: Más robusto
            datos.Direccion = ExtraerValorConRegex(datosAsegurado, @"Dirección:\s*([^L]+?)(?:\s+Localidad|$)", "").Trim();

            datos.Localidad = ExtraerValorConRegex(datosAsegurado, @"Localidad:\s*([^P]+?)(?:\s+País:|$)", "").Trim();
            datos.Departamento = ExtraerValorConRegex(datosAsegurado, @"Depto:\s*([^R]+?)(?:\s+RUT|$)", "").Trim();
        }

        private void ExtractVehiculoData(string datosVehiculo, SmartExtractedData datos)
        {
            datos.Marca = ExtraerValorConRegex(datosVehiculo, @"MARCA\s*:\s*([^M]+?)(?:\s+MARCA AGRUPADOS|$)", "").Trim();
            if (string.IsNullOrEmpty(datos.Marca))
                datos.Marca = "RENAULT"; // Fallback

            // PROBLEMA: Modelo vacío
            // ANTERIOR: @"MODELO\s*:\s*([^M]+?)(?:\s+MOTOR|$)"
            // NUEVO: Más específico y robusto
            datos.Modelo = ExtraerValorConRegex(datosVehiculo, @"MODELO\s*:\s*([^M]+?)(?:\s+MOTOR\s*:|$)", "").Trim();

            // Si no funciona, probar alternativa
            if (string.IsNullOrEmpty(datos.Modelo))
            {
                datos.Modelo = ExtraerValorConRegex(datosVehiculo, @"MODELO\s*:\s*(.+?)(?:\s+MOTOR|$)", "").Trim();
            }

            // PROBLEMA: Motor vacío
            // ANTERIOR: @"MOTOR\s*:\s*([^C]+?)(?:\s+CHASIS|$)"
            // NUEVO: Más específico
            datos.Motor = ExtraerValorConRegex(datosVehiculo, @"MOTOR\s*:\s*([^C\s]+)(?:\s+CHASIS|$)", "").Trim();

            datos.Chasis = ExtraerValorConRegex(datosVehiculo, @"CHASIS\s*:\s*([^D\s]+)(?:\s+DESTINO|$)", "").Trim();
            datos.Anio = ExtraerValorConRegex(datosVehiculo, @"AÑO\s*:\s*(\d{4})", "");
            datos.Combustible = ExtraerValorConRegex(datosVehiculo, @"COMBUSTIBLE\s*:\s*([^M]+?)(?:\s+MODELO|$)", "").Trim();
            datos.Matricula = ExtraerValorConRegex(datosVehiculo, @"MATRÍCULA\s*:\s*([^P]+?)(?:\s+PADRÓN|$)", "").Trim();

            if (string.IsNullOrEmpty(datos.Matricula))
                datos.Matricula = ExtraerValorConRegex(datosVehiculo, @"PADRÓN\s*:\s*([^D]+?)(?:\s+DESTINO|$)", "").Trim();
        }

        private void ExtractCorredorData(string datosCorredor, SmartExtractedData datos)
        {
            // Nombre corredor: "Nombre: ZABARI S A Número: 21077"
            datos.Corredor = ExtraerValorConRegex(datosCorredor, @"Nombre:\s*([^N]+?)(?:\s+Número:|$)", "").Trim();
        }

        #endregion

        #region Métodos de Extracción de Pago

        private void ExtractDatosPago(Dictionary<string, string> campos, SmartExtractedData datos)
        {
            try
            {
                _logger.LogDebug("💳 Extrayendo datos de pago...");

                // 1. BUSCAR FORMA DE PAGO EN MÚLTIPLES CAMPOS
                var formaPagoEncontrada = BuscarFormaPagoEnCampos(campos);
                if (!string.IsNullOrEmpty(formaPagoEncontrada))
                {
                    datos.FormaPago = formaPagoEncontrada;
                    _logger.LogDebug("✅ Forma de pago encontrada: {FormaPago}", formaPagoEncontrada);
                }

                // 2. BUSCAR CUOTAS EN MÚLTIPLES CAMPOS
                var cuotasEncontradas = BuscarCuotasEnCampos(campos);
                if (cuotasEncontradas > 0)
                {
                    datos.CantidadCuotas = cuotasEncontradas;
                    _logger.LogDebug("✅ Cuotas encontradas: {Cuotas}", cuotasEncontradas);
                }

                // 3. ESTABLECER RELACIONES LÓGICAS
                AjustarRelacionFormaPagoCuotas(datos);

                // ✅ AGREGAR ESTAS LÍNEAS NUEVAS:
                // 4. LIMPIAR FORMA DE PAGO
                if (!string.IsNullOrEmpty(datos.FormaPago))
                {
                    datos.FormaPago = LimpiarFormaPago(datos.FormaPago);
                    _logger.LogDebug("🧹 Forma de pago limpiada: {FormaPago}", datos.FormaPago);
                }

                // 5. EXTRAER CUOTAS DETALLADAS
                ExtractCuotasDetalladas(campos, datos);

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo datos de pago");
            }
        }

        private string LimpiarFormaPago(string formaPago)
        {
            if (string.IsNullOrEmpty(formaPago)) return "";

            // Remover prefijos comunes
            var prefijosARemover = new[]
            {
        "MEDIO DE PAGO:",
        "FORMA DE PAGO:",
        "MODALIDAD:",
        "PAGO:"
    };

            var resultado = formaPago;
            foreach (var prefijo in prefijosARemover)
            {
                if (resultado.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
                {
                    resultado = resultado.Substring(prefijo.Length).Trim();
                }
            }

            // Remover saltos de línea y espacios extras
            resultado = resultado.Replace("\n", " ").Replace("\r", " ").Trim();

            // Remover espacios múltiples
            while (resultado.Contains("  "))
            {
                resultado = resultado.Replace("  ", " ");
            }

            // Mapear valores comunes a estándares
            var mapeoFormasPago = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {"TARJETA DE CREDITO", "TARJETA DE CRÉDITO"},
        {"TARJETA DE DEBITO", "TARJETA DE DÉBITO"},
        {"TRANSFERENCIA BANCARIA", "TRANSFERENCIA"},
        {"EFECTIVO", "CONTADO"},
        {"CHEQUE", "CHEQUE"},
        {"DEBITO AUTOMATICO", "DÉBITO AUTOMÁTICO"}
    };

            if (mapeoFormasPago.ContainsKey(resultado))
            {
                return mapeoFormasPago[resultado];
            }

            return resultado.ToUpperInvariant();
        }


        private string BuscarFormaPagoEnCampos(Dictionary<string, string> campos)
        {
            // Lista de campos donde buscar forma de pago
            var camposFormaPago = new[]
            {
        "pago.medio", "medio_pago", "payment_method", "forma_pago",
        "modo_pago", "tipo_pago", "metodo_pago", "modalidad_pago",
        "pago.modo_facturacion"
    };

            foreach (var campo in camposFormaPago)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    var valor = campos[campo].ToLowerInvariant().Trim();

                    // Mapear a formas de pago estándar
                    if (valor.Contains("mensual") || valor.Contains("12"))
                        return "MENSUAL";
                    else if (valor.Contains("anual") || valor.Contains("1"))
                        return "ANUAL";
                    else if (valor.Contains("semestral") || valor.Contains("6"))
                        return "SEMESTRAL";
                    else if (valor.Contains("trimestral") || valor.Contains("3"))
                        return "TRIMESTRAL";
                    else if (valor.Contains("contado"))
                        return "CONTADO";
                    else if (!string.IsNullOrEmpty(valor) && valor.Length > 2)
                        return valor.ToUpperInvariant();
                }
            }

            // Buscar en TODO el contenido de todos los campos con regex
            foreach (var kvp in campos)
            {
                var texto = kvp.Value;

                // Patrones para detectar forma de pago
                var patrones = new[]
                {
            @"(?i)MEDIO\s+DE\s+PAGO[:\s]*([^\n\r.]{3,30})",
            @"(?i)forma\s+de\s+pago[:\s]*([^\n\r.]{3,30})",
            @"(?i)modalidad[:\s]*([^\n\r.]{3,30})",
            @"(?i)(mensual|anual|semestral|trimestral|contado)(?=\s|$|\n|\r)",
            @"(\d{1,2})\s*cuotas?"
        };

                foreach (var patron in patrones)
                {
                    var match = Regex.Match(texto, patron);
                    if (match.Success)
                    {
                        var valor = match.Groups[1].Value.ToLowerInvariant().Trim();

                        if (valor.Contains("mensual") || (int.TryParse(valor, out int num) && num == 12))
                            return "MENSUAL";
                        else if (valor.Contains("anual") || (int.TryParse(valor, out int num2) && num2 == 1))
                            return "ANUAL";
                        else if (valor.Contains("semestral") || (int.TryParse(valor, out int num3) && num3 == 6))
                            return "SEMESTRAL";
                        else if (valor.Contains("trimestral") || (int.TryParse(valor, out int num4) && num4 == 3))
                            return "TRIMESTRAL";
                        else if (valor.Contains("contado"))
                            return "CONTADO";
                        else if (!string.IsNullOrEmpty(valor) && valor.Length > 2)
                            return valor.ToUpperInvariant();
                    }
                }
            }

            return "";
        }

        private int BuscarCuotasEnCampos(Dictionary<string, string> campos)
        {
            // Lista de campos donde buscar cuotas
            var camposCuotas = new[]
            {
        "pago.modo_facturacion", "cuotas", "installments", "numero_cuotas",
        "cantidad_cuotas", "cant_cuotas", "modo_pago", "pago.cuotas"
    };

            foreach (var campo in camposCuotas)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    var cuotas = ExtraerNumeroCuotasDeTexto(campos[campo]);
                    if (cuotas > 0)
                        return cuotas;
                }
            }

            // Buscar en TODO el contenido con regex
            foreach (var kvp in campos)
            {
                var cuotas = ExtraerNumeroCuotasDeTexto(kvp.Value);
                if (cuotas > 0)
                    return cuotas;
            }

            return 1; // Default: 1 cuota
        }

        private int ExtraerNumeroCuotasDeTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return 0;

            var patrones = new[]
            {
        @"(\d{1,2})\s*cuotas?",
        @"cuotas?\s*[:\-]\s*(\d{1,2})",
        @"pago\s+en\s+(\d{1,2})",
        @"(\d{1,2})\s*mensual",
        @"mensual\s*(\d{1,2})"
    };

            foreach (var patron in patrones)
            {
                var match = Regex.Match(texto, patron, RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
                {
                    // Validar que sea un número razonable de cuotas (1-48)
                    if (cuotas >= 1 && cuotas <= 48)
                        return cuotas;
                }
            }

            // Casos especiales
            var textoLower = texto.ToLowerInvariant();
            if (textoLower.Contains("anual") || textoLower.Contains("contado"))
                return 1;
            else if (textoLower.Contains("mensual"))
                return 12; // Default para mensual
            else if (textoLower.Contains("semestral"))
                return 6;
            else if (textoLower.Contains("trimestral"))
                return 3;

            return 0;
        }

        private void AjustarRelacionFormaPagoCuotas(SmartExtractedData datos)
        {
            // Establecer relaciones lógicas entre forma de pago y cuotas
            if (!string.IsNullOrEmpty(datos.FormaPago))
            {
                switch (datos.FormaPago.ToUpperInvariant())
                {
                    case "MENSUAL":
                        if (datos.CantidadCuotas <= 1) datos.CantidadCuotas = 12;
                        break;
                    case "ANUAL":
                    case "CONTADO":
                        if (datos.CantidadCuotas > 1) datos.CantidadCuotas = 1;
                        break;
                    case "SEMESTRAL":
                        if (datos.CantidadCuotas <= 1) datos.CantidadCuotas = 6;
                        break;
                    case "TRIMESTRAL":
                        if (datos.CantidadCuotas <= 1) datos.CantidadCuotas = 3;
                        break;
                }
            }
            else if (datos.CantidadCuotas > 1)
            {
                // Inferir forma de pago basado en cuotas
                datos.FormaPago = datos.CantidadCuotas switch
                {
                    1 => "CONTADO",
                    3 => "TRIMESTRAL",
                    6 => "SEMESTRAL",
                    12 => "MENSUAL",
                    _ => "MENSUAL"
                };
            }
        }

        #endregion

        #region Métodos de Extracción Adicionales

        private void ExtractDatosAdicionales(Dictionary<string, string> campos, SmartExtractedData datos)
        {
            try
            {
                // Teléfono
                foreach (var kvp in campos)
                {
                    var telefono = ExtraerTelefonoDeTexto(kvp.Value);
                    if (!string.IsNullOrEmpty(telefono))
                    {
                        datos.Telefono = telefono;
                        break;
                    }
                }

                // Color del vehículo
                var colores = new[] { "blanco", "negro", "gris", "plata", "azul", "rojo", "verde", "amarillo", "beige" };
                foreach (var kvp in campos)
                {
                    foreach (var color in colores)
                    {
                        if (kvp.Value.Contains(color, StringComparison.OrdinalIgnoreCase))
                        {
                            datos.Color = color.ToUpperInvariant();
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(datos.Color)) break;
                }

                // Uso del vehículo
                var usos = new[] { "particular", "comercial", "taxi", "remise", "carga", "transporte" };
                foreach (var kvp in campos)
                {
                    foreach (var uso in usos)
                    {
                        if (kvp.Value.Contains(uso, StringComparison.OrdinalIgnoreCase))
                        {
                            datos.Uso = uso.ToUpperInvariant();
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(datos.Uso)) break;
                }

                // Código postal
                foreach (var kvp in campos)
                {
                    var cp = ExtraerCodigoPostal(kvp.Value);
                    if (!string.IsNullOrEmpty(cp))
                    {
                        datos.CodigoPostal = cp;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo datos adicionales");
            }
        }

        private string ExtraerTelefonoDeTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";

            var patterns = new[]
            {
        @"(\+?598\s*\d{8,9})", // Formato uruguayo
        @"(?i)tel[éefonosfax:\s]*(\+?598\s*\d{8,9})",
        @"(?i)teléfono[:\s]*(\+?598\s*\d{8,9})",
        @"(\d{8,9})" // Solo números
    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(texto, pattern);
                if (match.Success)
                {
                    var telefono = match.Groups[1].Value.Trim();
                    // Validar que tenga al menos 8 dígitos
                    if (Regex.IsMatch(telefono, @"\d{8,}"))
                        return telefono;
                }
            }

            return "";
        }

        private string ExtraerCodigoPostal(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";

            var patterns = new[]
            {
        @"(?i)código\s+postal[:\s]*(\d{5})",
        @"(?i)cp[:\s]*(\d{5})",
        @"(\d{5})(?=\s|$|\n)"
    };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(texto, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return "";
        }

        private void ExtractCuotasDetalladas(Dictionary<string, string> campos, SmartExtractedData datos)
        {
            try
            {
                _logger.LogDebug("📅 Extrayendo cuotas detalladas...");

                // Log campos que pueden contener cuotas
                var camposRelevantes = campos.Where(kvp =>
                    kvp.Key.Contains("cuota", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Contains("pago", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Contains("vencimiento", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Contains("prima", StringComparison.OrdinalIgnoreCase)
                ).ToList();

                _logger.LogDebug("🔍 Campos relevantes para cuotas encontrados: {Count}", camposRelevantes.Count);
                foreach (var campo in camposRelevantes.Take(5)) // Solo los primeros 5 para no saturar logs
                {
                    _logger.LogDebug("📄 Campo: {Key} = {Value}", campo.Key,
                        campo.Value.Substring(0, Math.Min(50, campo.Value.Length)));
                }

                // 1. BUSCAR TABLA DE CUOTAS ESPECÍFICA
                var tablaCuotasEncontrada = BuscarTablaCuotas(campos);
                if (tablaCuotasEncontrada.Any())
                {
                    datos.DetalleCuotas.Cuotas = tablaCuotasEncontrada;
                    datos.DetalleCuotas.CantidadTotal = tablaCuotasEncontrada.Count;
                    datos.CantidadCuotas = Math.Max(datos.CantidadCuotas, tablaCuotasEncontrada.Count);
                    _logger.LogInformation("✅ Tabla de cuotas encontrada: {Count} cuotas detalladas", tablaCuotasEncontrada.Count);

                    // Log de la primera cuota para verificar
                    var primera = tablaCuotasEncontrada.FirstOrDefault();
                    if (primera != null)
                    {
                        _logger.LogInformation("📅 Primera cuota: #{Numero} - {Fecha} - ${Monto}",
                            primera.Numero,
                            primera.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "SIN FECHA",
                            primera.Monto);
                    }
                }

                // 2. BUSCAR INFORMACIÓN DE PRIMERA CUOTA
                if (!datos.DetalleCuotas.TieneCuotasDetalladas)
                {
                    var primeraCuota = BuscarPrimeraCuotaEspecifica(campos);
                    if (primeraCuota != null)
                    {
                        datos.DetalleCuotas.Cuotas.Add(primeraCuota);
                        datos.DetalleCuotas.CantidadTotal = Math.Max(datos.CantidadCuotas, 1);
                        _logger.LogDebug("✅ Primera cuota encontrada: {Fecha} - ${Monto}",
                            primeraCuota.FechaVencimiento?.ToString("dd/MM/yyyy"), primeraCuota.Monto);
                    }
                }

                // 3. ACTUALIZAR INFORMACIÓN CONSOLIDADA
                datos.DetalleCuotas.CantidadTotal = Math.Max(datos.CantidadCuotas, datos.DetalleCuotas.Cuotas.Count);

                _logger.LogInformation("📊 Resultado cuotas detalladas: Total={Total}, Detalladas={Detalladas}, TieneDatos={TieneDatos}",
                    datos.DetalleCuotas.CantidadTotal,
                    datos.DetalleCuotas.Cuotas.Count,
                    datos.DetalleCuotas.TieneCuotasDetalladas);

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo cuotas detalladas");
            }
        }

        private List<DetalleCuota> BuscarTablaCuotas(Dictionary<string, string> campos)
        {
            var cuotas = new List<DetalleCuota>();

            // ✅ AGREGAR: BUSCAR CAMPOS ESPECÍFICOS DE AZURE DOCUMENT INTELLIGENCE
            var cuotasAzure = ExtraerCuotasDeAzure(campos);
            if (cuotasAzure.Any())
            {
                cuotas.AddRange(cuotasAzure);
                _logger.LogDebug("✅ Cuotas extraídas de campos Azure: {Count}", cuotasAzure.Count);
            }

            // BUSCAR CAMPOS QUE CONTENGAN TABLAS DE CUOTAS
            var camposCuotasTabla = campos.Where(kvp =>
                kvp.Key.Contains("cuotas", StringComparison.OrdinalIgnoreCase) ||
                kvp.Key.Contains("tabla", StringComparison.OrdinalIgnoreCase) ||
                kvp.Key.Contains("schedule", StringComparison.OrdinalIgnoreCase) ||
                kvp.Key.Contains("payment", StringComparison.OrdinalIgnoreCase) ||
                kvp.Value.Contains("CUOTA", StringComparison.OrdinalIgnoreCase) ||
                kvp.Value.Contains("VENCIMIENTO", StringComparison.OrdinalIgnoreCase) ||
                kvp.Value.Contains("IMPORTE", StringComparison.OrdinalIgnoreCase)
            ).ToList();

            foreach (var campo in camposCuotasTabla)
            {
                var cuotasExtraidas = ParsearTablaCuotas(campo.Value);
                cuotas.AddRange(cuotasExtraidas);
            }

            // ORDENAR POR NÚMERO DE CUOTA O FECHA
            return cuotas.OrderBy(c => c.Numero).ThenBy(c => c.FechaVencimiento).ToList();
        }

        private List<DetalleCuota> ExtraerCuotasDeAzure(Dictionary<string, string> campos)
        {
            var cuotas = new List<DetalleCuota>();

            try
            {
                _logger.LogDebug("🔍 Buscando campos específicos de Azure Document Intelligence...");

                // PATRÓN 1: Campos individuales tipo "pago.cuotas[1].vencimiento"
                var camposCuotasIndividuales = campos.Where(kvp =>
                    kvp.Key.Contains("pago.cuotas[", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Contains("cuotas[", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.Contains("installment", StringComparison.OrdinalIgnoreCase)
                ).ToList();

                // Agrupar por número de cuota
                var cuotasAgrupadas = new Dictionary<int, DetalleCuota>();

                foreach (var campo in camposCuotasIndividuales)
                {
                    var numeroCuota = ExtraerNumeroCuotaDeCampo(campo.Key);
                    if (numeroCuota > 0)
                    {
                        if (!cuotasAgrupadas.ContainsKey(numeroCuota))
                        {
                            cuotasAgrupadas[numeroCuota] = new DetalleCuota
                            {
                                Numero = numeroCuota,
                                Estado = "PENDIENTE"
                            };
                        }

                        // Extraer vencimiento
                        if (campo.Key.Contains("vencimiento", StringComparison.OrdinalIgnoreCase) ||
                            campo.Key.Contains("fecha", StringComparison.OrdinalIgnoreCase) ||
                            campo.Key.Contains("due", StringComparison.OrdinalIgnoreCase))
                        {
                            var fecha = ExtraerFechaDeCampo(campo.Value);
                            if (fecha.HasValue)
                            {
                                cuotasAgrupadas[numeroCuota].FechaVencimiento = fecha;
                                _logger.LogDebug("📅 Cuota {Numero}: Vencimiento {Fecha}", numeroCuota, fecha.Value.ToString("dd/MM/yyyy"));
                            }
                        }

                        // Extraer monto
                        if (campo.Key.Contains("prima", StringComparison.OrdinalIgnoreCase) ||
                            campo.Key.Contains("monto", StringComparison.OrdinalIgnoreCase) ||
                            campo.Key.Contains("amount", StringComparison.OrdinalIgnoreCase) ||
                            campo.Key.Contains("importe", StringComparison.OrdinalIgnoreCase))
                        {
                            var monto = ExtraerMontoDeCampo(campo.Value);
                            if (monto > 0)
                            {
                                cuotasAgrupadas[numeroCuota].Monto = monto;
                                _logger.LogDebug("💰 Cuota {Numero}: Monto ${Monto}", numeroCuota, monto);
                            }
                        }
                    }
                }

                // Convertir a lista y validar
                cuotas.AddRange(cuotasAgrupadas.Values.Where(c =>
                    c.FechaVencimiento.HasValue || c.Monto > 0));

                _logger.LogDebug("✅ Extraídas {Count} cuotas de campos Azure específicos", cuotas.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo cuotas de campos Azure");
            }

            return cuotas;
        }

        private int ExtraerNumeroCuotaDeCampo(string nombreCampo)
        {
            // Buscar patrones como "pago.cuotas[1]" o "cuotas[2]"
            var match = Regex.Match(nombreCampo, @"cuotas\[(\d+)\]", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int numero))
            {
                return numero;
            }

            // Buscar patrones como "installment_1" o "cuota_2"
            match = Regex.Match(nombreCampo, @"(?:cuota|installment)_?(\d+)", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out numero))
            {
                return numero;
            }

            return 0;
        }

        private DateTime? ExtraerFechaDeCampo(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return null;

            // Limpiar el valor
            var fechaLimpia = valor.Replace("Vencimiento:", "").Replace("Fecha:", "").Trim();

            // Formatos de fecha uruguayos
            var formatos = new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy" };

            foreach (var formato in formatos)
            {
                if (DateTime.TryParseExact(fechaLimpia, formato, null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                {
                    return fecha;
                }
            }

            // Intentar parsing general
            if (DateTime.TryParse(fechaLimpia, out DateTime fechaGeneral))
            {
                return fechaGeneral;
            }

            return null;
        }

        private decimal ExtraerMontoDeCampo(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return 0;

            // Limpiar el valor
            var montoLimpio = valor.Replace("Prima:", "").Replace("$", "").Replace("USD", "").Replace("UYU", "").Trim();

            // Formato uruguayo: 15.379,00 (punto como separador de miles, coma como decimal)
            if (montoLimpio.Contains(","))
            {
                montoLimpio = montoLimpio.Replace(".", "").Replace(",", ".");
            }

            if (decimal.TryParse(montoLimpio, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal monto))
            {
                return monto;
            }

            return 0;
        }

        private List<DetalleCuota> ParsearTablaCuotas(string textoTabla)
        {
            var cuotas = new List<DetalleCuota>();

            if (string.IsNullOrEmpty(textoTabla)) return cuotas;

            try
            {
                // PATRÓN PRINCIPAL: Buscar líneas que contengan información de cuotas
                // Ejemplos del gist: "1 22/04/2024 $ 15.379,00"
                var patronCuota = @"(\d{1,2})\s+(\d{1,2}/\d{1,2}/\d{4})\s+\$?\s*([\d,.]+)";
                var matches = Regex.Matches(textoTabla, patronCuota, RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    if (int.TryParse(match.Groups[1].Value, out int numeroCuota) &&
                        DateTime.TryParseExact(match.Groups[2].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                    {
                        var montoStr = match.Groups[3].Value.Replace(".", "").Replace(",", ".");
                        if (decimal.TryParse(montoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal monto))
                        {
                            cuotas.Add(new DetalleCuota
                            {
                                Numero = numeroCuota,
                                FechaVencimiento = fecha,
                                Monto = monto,
                                Estado = "PENDIENTE"
                            });
                        }
                    }
                }

                // PATRÓN ALTERNATIVO: Buscar formato más libre
                if (!cuotas.Any())
                {
                    var lineas = textoTabla.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var linea in lineas)
                    {
                        var cuota = ParsearLineaCuota(linea);
                        if (cuota != null)
                        {
                            cuotas.Add(cuota);
                        }
                    }
                }

                _logger.LogDebug("📊 Parseadas {Count} cuotas de tabla", cuotas.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parseando tabla de cuotas");
            }

            return cuotas;
        }

        private DetalleCuota ParsearLineaCuota(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea)) return null;

            try
            {
                // VARIOS PATRONES PARA DETECTAR CUOTAS
                var patrones = new[]
                {
            @"(\d{1,2})\s+(\d{1,2}/\d{1,2}/\d{4})\s+\$?\s*([\d,.]+)",  // "1 22/04/2024 $ 15.379,00"
            @"CUOTA\s*(\d{1,2})[:\s]+(\d{1,2}/\d{1,2}/\d{4})[:\s]+\$?\s*([\d,.]+)", // "CUOTA 1: 22/04/2024: $ 15.379,00"
            @"(\d{1,2})[^\d]*(\d{1,2}/\d{1,2}/\d{4})[^\d]*([\d,.]+)", // Formato más flexible
        };

                foreach (var patron in patrones)
                {
                    var match = Regex.Match(linea, patron, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        if (int.TryParse(match.Groups[1].Value, out int numero) &&
                            DateTime.TryParseExact(match.Groups[2].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                        {
                            var montoStr = match.Groups[3].Value.Replace(".", "").Replace(",", ".");
                            if (decimal.TryParse(montoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal monto))
                            {
                                return new DetalleCuota
                                {
                                    Numero = numero,
                                    FechaVencimiento = fecha,
                                    Monto = monto,
                                    Estado = "PENDIENTE"
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Error parseando línea de cuota: {Error}", ex.Message);
            }

            return null;
        }

        private DetalleCuota BuscarPrimeraCuotaEspecifica(Dictionary<string, string> campos)
        {
            // BUSCAR CAMPOS ESPECÍFICOS DE PRIMERA CUOTA
            var camposPrimeraCuota = new[]
            {
        "primera_cuota", "primer_vencimiento", "cuota_1", "first_payment",
        "proximo_vencimiento", "primer_pago"
    };

            foreach (var campo in camposPrimeraCuota)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    var cuota = ParsearLineaCuota(campos[campo]);
                    if (cuota != null)
                    {
                        cuota.Numero = 1; // Asegurar que es la primera
                        return cuota;
                    }
                }
            }

            // BUSCAR EN TODOS LOS CAMPOS CON PATRONES ESPECÍFICOS
            foreach (var kvp in campos)
            {
                if (kvp.Value.Contains("PRIMER", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Value.Contains("PRÓXIM", StringComparison.OrdinalIgnoreCase) ||
                    kvp.Value.Contains("VENCIMIENTO", StringComparison.OrdinalIgnoreCase))
                {
                    // Buscar patrón de fecha + monto
                    var match = Regex.Match(kvp.Value, @"(\d{1,2}/\d{1,2}/\d{4})\s+\$?\s*([\d,.]+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        if (DateTime.TryParseExact(match.Groups[1].Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                        {
                            var montoStr = match.Groups[2].Value.Replace(".", "").Replace(",", ".");
                            if (decimal.TryParse(montoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal monto))
                            {
                                return new DetalleCuota
                                {
                                    Numero = 1,
                                    FechaVencimiento = fecha,
                                    Monto = monto,
                                    Estado = "PENDIENTE"
                                };
                            }
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Métodos Auxiliares

        private string ExtraerValorConRegex(string texto, string patron, string valorPorDefecto = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(texto)) return valorPorDefecto;

                var match = Regex.Match(texto, patron, RegexOptions.IgnoreCase);
                return match.Success ? match.Groups[1].Value.Trim() : valorPorDefecto;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error extrayendo con regex '{Patron}': {Error}", patron, ex.Message);
                return valorPorDefecto;
            }
        }

        private decimal ParsearNumero(string numero)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numero)) return 0;

                // Limpiar el número: quitar puntos (separadores de miles) y reemplazar coma por punto decimal
                var numeroLimpio = numero.Replace(".", "").Replace(",", ".");

                return decimal.TryParse(numeroLimpio, System.Globalization.NumberStyles.Any,
                                       System.Globalization.CultureInfo.InvariantCulture, out var resultado) ? resultado : 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error parseando número '{Numero}': {Error}", numero, ex.Message);
                return 0;
            }
        }

        // DTO para los datos extraídos inteligentemente
        public class SmartExtractedData
        {
            public string NumeroPoliza { get; set; } = "";
            public string Asegurado { get; set; } = "";
            public string Documento { get; set; } = "";
            public string Vehiculo { get; set; } = "";
            public string Marca { get; set; } = "";
            public string Modelo { get; set; } = "";
            public string Matricula { get; set; } = "";
            public string Motor { get; set; } = "";
            public string Chasis { get; set; } = "";
            public decimal PrimaComercial { get; set; } = 0;
            public decimal PremioTotal { get; set; } = 0;
            public DateTime? VigenciaDesde { get; set; }
            public DateTime? VigenciaHasta { get; set; }
            public string Corredor { get; set; } = "";
            public string Plan { get; set; } = "";
            public string Ramo { get; set; } = "AUTOMOVILES";
            public string Anio { get; set; } = "";
            public string Combustible { get; set; } = "";
            public string Email { get; set; } = "";
            public string Direccion { get; set; } = "";
            public string Departamento { get; set; } = "";
            public string Localidad { get; set; } = "";

            public string FormaPago { get; set; } = "";
            public int CantidadCuotas { get; set; } = 1;
            public string TipoVehiculo { get; set; } = "";
            public string Uso { get; set; } = "";
            public string Color { get; set; } = "";
            public string Telefono { get; set; } = "";
            public string CodigoPostal { get; set; } = "";
            public decimal ImpuestoMSP { get; set; } = 0;
            public decimal Descuentos { get; set; } = 0;
            public decimal Recargos { get; set; } = 0;

            public InformacionCuotas DetalleCuotas { get; set; } = new();
            public DateTime? PrimerVencimiento => DetalleCuotas.PrimeraCuota?.FechaVencimiento;
            public decimal PrimaCuota => DetalleCuotas.PrimeraCuota?.Monto ?? 0;
        }

        public class DetalleCuota
        {
            public int Numero { get; set; }
            public DateTime? FechaVencimiento { get; set; }
            public decimal Monto { get; set; }
            public string Estado { get; set; } = "";
        }

        public class InformacionCuotas
        {
            public int CantidadTotal { get; set; }
            public List<DetalleCuota> Cuotas { get; set; } = new();
            public DetalleCuota PrimeraCuota => Cuotas.FirstOrDefault();
            public decimal MontoPromedio => Cuotas.Any() ? Cuotas.Average(c => c.Monto) : 0;
            public bool TieneCuotasDetalladas => Cuotas.Any();
        }

    #endregion

    public AzureDatosPolizaVelneoDto ExtraerDatosOrganizadosVelneo(Dictionary<string, string> camposExtraidos)
        {
            var datosVelneo = new AzureDatosPolizaVelneoDto();

            try
            {
                _logger.LogInformation("🏗️ Iniciando extracción organizada para Velneo de {CamposCount} campos", camposExtraidos.Count);

                // Primero extraer con el método existente para mantener compatibilidad
                var datosLegacy = ExtraerDatosInteligente(camposExtraidos);

                // Luego organizar en las nuevas secciones
                datosVelneo.DatosBasicos = ExtraerDatosBasicos(camposExtraidos, datosLegacy);
                datosVelneo.DatosPoliza = ExtraerDatosPoliza(camposExtraidos, datosLegacy);
                datosVelneo.DatosVehiculo = ExtraerDatosVehiculo(camposExtraidos, datosLegacy);
                datosVelneo.DatosCobertura = ExtraerDatosCobertura(camposExtraidos, datosLegacy);
                datosVelneo.CondicionesPago = ExtraerCondicionesPago(camposExtraidos, datosLegacy);
                datosVelneo.Bonificaciones = ExtraerBonificaciones(camposExtraidos, datosLegacy);
                datosVelneo.Observaciones = ExtraerObservaciones(camposExtraidos, datosLegacy);
                datosVelneo.Metricas = CalcularMetricas(datosVelneo, camposExtraidos.Count);

                _logger.LogInformation("✅ Extracción organizada completada: {Completitud}% completitud",
                    datosVelneo.Metricas.PorcentajeCompletitud);

                return datosVelneo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en extracción organizada");
                return datosVelneo; // Retorna datos parciales
            }
        }

        #region Extracción por Secciones Velneo

        private AzureDatosBasicosDto ExtraerDatosBasicos(Dictionary<string, string> campos, SmartExtractedData datosLegacy)
        {
            var datos = new AzureDatosBasicosDto();

            try
            {
                _logger.LogDebug("👤 Extrayendo Datos Básicos...");

                // Mapear desde datos legacy
                datos.Asegurado = datosLegacy.Asegurado;
                datos.Domicilio = datosLegacy.Direccion;
                datos.Telefono = datosLegacy.Telefono;
                datos.Email = datosLegacy.Email;
                datos.Documento = datosLegacy.Documento;
                datos.Departamento = datosLegacy.Departamento;
                datos.Localidad = datosLegacy.Localidad;
                datos.CodigoPostal = datosLegacy.CodigoPostal;
                datos.Corredor = datosLegacy.Corredor;

                // Extraer fecha de emisión
                if (datosLegacy.VigenciaDesde.HasValue)
                {
                    datos.Fecha = datosLegacy.VigenciaDesde; // Usar vigencia como fecha de emisión por defecto
                }

                // Determinar tipo (Persona/Empresa) basado en el documento
                datos.Tipo = DeterminarTipoCliente(datos.Documento, datos.Asegurado);

                // Buscar trámite específico
                datos.Tramite = BuscarTramite(campos);

                _logger.LogDebug("✅ Datos Básicos: Cliente={Cliente}, Tipo={Tipo}", datos.Asegurado, datos.Tipo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo datos básicos");
            }

            return datos;
        }

        // ✅ SECCIÓN 2: DATOS DE LA PÓLIZA
        private AzureDatosPolizaDto ExtraerDatosPoliza(Dictionary<string, string> campos, SmartExtractedData datosLegacy)
        {
            var datos = new AzureDatosPolizaDto();

            try
            {
                _logger.LogDebug("📋 Extrayendo Datos de la Póliza...");

                // Mapear desde datos legacy
                datos.NumeroPoliza = datosLegacy.NumeroPoliza;
                datos.Desde = datosLegacy.VigenciaDesde;
                datos.Hasta = datosLegacy.VigenciaHasta;
                datos.Ramo = datosLegacy.Ramo;

                // Buscar campos específicos
                datos.Certificado = BuscarCertificado(campos);
                datos.Endoso = BuscarEndoso(campos);
                datos.TipoMovimiento = BuscarTipoMovimiento(campos);

                _logger.LogDebug("✅ Datos Póliza: Número={Numero}, Vigencia={Desde}-{Hasta}",
                    datos.NumeroPoliza, datos.Desde?.ToString("dd/MM/yyyy"), datos.Hasta?.ToString("dd/MM/yyyy"));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo datos de póliza");
            }

            return datos;
        }

        // ✅ SECCIÓN 3: DATOS DEL VEHÍCULO
        private AzureDatosVehiculoDto ExtraerDatosVehiculo(Dictionary<string, string> campos, SmartExtractedData datosLegacy)
        {
            var datos = new AzureDatosVehiculoDto();

            try
            {
                _logger.LogDebug("🚗 Extrayendo Datos del Vehículo...");

                // Mapear desde datos legacy
                datos.Marca = datosLegacy.Marca;
                datos.Modelo = datosLegacy.Modelo;
                datos.MarcaModelo = $"{datos.Marca} {datos.Modelo}".Trim();
                datos.Anio = datosLegacy.Anio;
                datos.Motor = datosLegacy.Motor;
                datos.Combustible = datosLegacy.Combustible;
                datos.Chasis = datosLegacy.Chasis;
                datos.Matricula = datosLegacy.Matricula;
                datos.Color = datosLegacy.Color;
                datos.TipoVehiculo = datosLegacy.TipoVehiculo;
                datos.Uso = datosLegacy.Uso;

                // Mapear uso a destino si es posible
                datos.Destino = MapearUsoADestino(datos.Uso);

                // Determinar categoría basada en tipo de vehículo
                datos.Categoria = DeterminarCategoria(datos.TipoVehiculo, datos.Uso);

                _logger.LogDebug("✅ Datos Vehículo: {Marca} {Modelo} {Anio}, Uso={Uso}",
                    datos.Marca, datos.Modelo, datos.Anio, datos.Uso);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo datos del vehículo");
            }

            return datos;
        }

        // ✅ SECCIÓN 4: DATOS DE LA COBERTURA
        private AzureDatosCoberturaDto ExtraerDatosCobertura(Dictionary<string, string> campos, SmartExtractedData datosLegacy)
        {
            var datos = new AzureDatosCoberturaDto();

            try
            {
                _logger.LogDebug("🛡️ Extrayendo Datos de la Cobertura...");

                // Extraer cobertura/plan
                datos.Cobertura = datosLegacy.Plan;

                // Mapear departamento a zona de circulación
                datos.ZonaCirculacion = datosLegacy.Departamento;

                // Determinar moneda y código
                datos.Moneda = DeterminarMoneda(campos);
                datos.CodigoMoneda = MapearCodigoMoneda(datos.Moneda);

                _logger.LogDebug("✅ Datos Cobertura: Plan={Plan}, Zona={Zona}, Moneda={Moneda}",
                    datos.Cobertura, datos.ZonaCirculacion, datos.Moneda);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo datos de cobertura");
            }

            return datos;
        }

        // ✅ SECCIÓN 5: CONDICIONES DE PAGO
        private AzureCondicionesPagoDto ExtraerCondicionesPago(Dictionary<string, string> campos, SmartExtractedData datosLegacy)
        {
            var datos = new AzureCondicionesPagoDto();

            try
            {
                _logger.LogDebug("💳 Extrayendo Condiciones de Pago...");

                // Mapear desde datos legacy
                datos.FormaPago = datosLegacy.FormaPago;
                datos.Premio = datosLegacy.PrimaComercial;
                datos.Total = datosLegacy.PremioTotal;
                datos.Cuotas = datosLegacy.CantidadCuotas;
                datos.DetalleCuotas = ConvertirInformacionCuotas(datosLegacy.DetalleCuotas);

                // Calcular valor cuota
                if (datos.Cuotas > 0 && datos.Total > 0)
                {
                    datos.ValorCuota = datos.Total / datos.Cuotas;
                }
                else if (datosLegacy.DetalleCuotas.TieneCuotasDetalladas)
                {
                    datos.ValorCuota = datosLegacy.DetalleCuotas.MontoPromedio;
                }

                // Determinar moneda
                datos.Moneda = DeterminarMoneda(campos);

                _logger.LogDebug("✅ Condiciones Pago: FormaPago={FormaPago}, Total={Total}, Cuotas={Cuotas}, ValorCuota={ValorCuota}",
                    datos.FormaPago, datos.Total, datos.Cuotas, datos.ValorCuota);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo condiciones de pago");
            }

            return datos;
        }

        // ✅ SECCIÓN 6: BONIFICACIONES
        private AzureBonificacionesDto ExtraerBonificaciones(Dictionary<string, string> campos, SmartExtractedData datosLegacy)
        {
            var datos = new AzureBonificacionesDto();

            try
            {
                _logger.LogDebug("🎁 Extrayendo Bonificaciones...");

                // Mapear desde datos legacy
                datos.Descuentos = datosLegacy.Descuentos;
                datos.Recargos = datosLegacy.Recargos;
                datos.ImpuestoMSP = datosLegacy.ImpuestoMSP;

                // Buscar bonificaciones específicas en los campos
                datos.Bonificaciones = BuscarBonificaciones(campos);
                datos.TotalBonificaciones = datos.Bonificaciones.Sum(b => b.Monto);

                _logger.LogDebug("✅ Bonificaciones: Descuentos={Descuentos}, Recargos={Recargos}, ImpuestoMSP={ImpuestoMSP}",
                    datos.Descuentos, datos.Recargos, datos.ImpuestoMSP);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo bonificaciones");
            }

            return datos;
        }

        // ✅ SECCIÓN 7: OBSERVACIONES
        private AzureObservacionesDto ExtraerObservaciones(Dictionary<string, string> campos, SmartExtractedData datosLegacy)
        {
            var datos = new AzureObservacionesDto();

            try
            {
                _logger.LogDebug("📝 Extrayendo Observaciones...");

                // Buscar observaciones en campos específicos
                datos.ObservacionesGenerales = BuscarObservaciones(campos);

                // Agregar notas del procesamiento
                datos.NotasEscaneado.Add($"Documento procesado automáticamente el {DateTime.Now:dd/MM/yyyy HH:mm}");

                if (datosLegacy.DetalleCuotas.TieneCuotasDetalladas)
                {
                    datos.NotasEscaneado.Add($"Cronograma de {datosLegacy.DetalleCuotas.Cuotas.Count} cuotas extraído automáticamente");
                }

                _logger.LogDebug("✅ Observaciones extraídas");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extrayendo observaciones");
            }

            return datos;
        }

        // ✅ MÉTRICAS DE CALIDAD
        private AzureMetricasExtraccionDto CalcularMetricas(AzureDatosPolizaVelneoDto datos, int totalCampos)
        {
            var metricas = new AzureMetricasExtraccionDto();

            try
            {
                // Contar campos completos por sección
                var camposCompletos = 0;
                var totalCamposEsperados = 0;

                // Datos Básicos (campos críticos)
                var camposBasicos = new[] { datos.DatosBasicos.Asegurado, datos.DatosBasicos.Documento, datos.DatosBasicos.Domicilio };
                camposCompletos += camposBasicos.Count(c => !string.IsNullOrEmpty(c));
                totalCamposEsperados += camposBasicos.Length;

                // Datos Póliza (campos críticos)
                var camposPoliza = new[] { datos.DatosPoliza.NumeroPoliza };
                camposCompletos += camposPoliza.Count(c => !string.IsNullOrEmpty(c));
                totalCamposEsperados += camposPoliza.Length;

                // Datos Vehículo (campos críticos)
                var camposVehiculo = new[] { datos.DatosVehiculo.Marca, datos.DatosVehiculo.Modelo, datos.DatosVehiculo.Anio };
                camposCompletos += camposVehiculo.Count(c => !string.IsNullOrEmpty(c));
                totalCamposEsperados += camposVehiculo.Length;

                // Condiciones Pago (campos críticos)
                var camposPago = new[] { datos.CondicionesPago.FormaPago };
                camposCompletos += camposPago.Count(c => !string.IsNullOrEmpty(c));
                totalCamposEsperados += camposPago.Length;

                metricas.CamposCompletos = camposCompletos;
                metricas.CamposExtraidos = totalCampos;
                metricas.PorcentajeCompletitud = totalCamposEsperados > 0 ?
                    (decimal)camposCompletos / totalCamposEsperados * 100 : 0;

                metricas.TieneDatosMinimos = !string.IsNullOrEmpty(datos.DatosPoliza.NumeroPoliza) &&
                                            !string.IsNullOrEmpty(datos.DatosBasicos.Asegurado) &&
                                            !string.IsNullOrEmpty(datos.DatosBasicos.Documento);

                // Identificar campos faltantes críticos
                if (string.IsNullOrEmpty(datos.DatosBasicos.Asegurado)) metricas.CamposFaltantes.Add("Asegurado");
                if (string.IsNullOrEmpty(datos.DatosBasicos.Documento)) metricas.CamposFaltantes.Add("Documento");
                if (string.IsNullOrEmpty(datos.DatosPoliza.NumeroPoliza)) metricas.CamposFaltantes.Add("Número de Póliza");
                if (string.IsNullOrEmpty(datos.DatosVehiculo.Marca)) metricas.CamposFaltantes.Add("Marca del Vehículo");

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error calculando métricas");
            }

            return metricas;
        }

        #endregion

        #region Métodos Auxiliares de Mapeo

        private string DeterminarTipoCliente(string documento, string asegurado)
        {
            // Si el documento es RUT (12 dígitos) y el nombre suena empresarial, es empresa
            if (!string.IsNullOrEmpty(documento) && documento.Length >= 12)
            {
                var nombresEmpresa = new[] { "SA", "S.A", "SRL", "S.R.L", "LTDA", "CIA", "EMPRESA", "SOCIEDAD" };
                if (nombresEmpresa.Any(emp => asegurado.Contains(emp, StringComparison.OrdinalIgnoreCase)))
                {
                    return "EMPRESA";
                }
            }
            return "PERSONA";
        }

        private string BuscarTramite(Dictionary<string, string> campos)
        {
            var camposTramite = new[] { "tramite", "tipo_tramite", "operacion", "movimiento" };

            foreach (var campo in camposTramite)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    return campos[campo];
                }
            }

            return "RENOVACION"; // Default
        }

        private string BuscarCertificado(Dictionary<string, string> campos)
        {
            var camposCertificado = new[] { "certificado", "poliza.certificado", "numero_certificado", "cert" };

            foreach (var campo in camposCertificado)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    var certificado = campos[campo];

                    // Limpiar formato: "Certificado Nº:\n1" -> "Nº 1"
                    certificado = certificado
                        .Replace("Certificado Nº:", "Nº")
                        .Replace("Certificado No:", "Nº")
                        .Replace("Certificado N°:", "Nº")
                        .Replace("Certificado:", "")
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Trim();

                    // Remover espacios múltiples
                    while (certificado.Contains("  "))
                    {
                        certificado = certificado.Replace("  ", " ");
                    }

                    _logger.LogDebug("✅ Certificado formateado: {Certificado}", certificado);
                    return certificado;
                }
            }

            return "";
        }

        private string BuscarEndoso(Dictionary<string, string> campos)
        {
            var camposEndoso = new[] { "endoso", "poliza.endoso", "numero_endoso" };

            foreach (var campo in camposEndoso)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    return campos[campo];
                }
            }

            return "0"; // Default
        }

        private string BuscarTipoMovimiento(Dictionary<string, string> campos)
        {
            // Buscar primero el campo específico de Azure Document Intelligence
            if (campos.ContainsKey("poliza.tipo_movimiento") && !string.IsNullOrEmpty(campos["poliza.tipo_movimiento"]))
            {
                var tipoMovimiento = campos["poliza.tipo_movimiento"].Trim().ToUpperInvariant();
                _logger.LogDebug("✅ Tipo movimiento encontrado en Azure: {TipoMovimiento}", tipoMovimiento);
                return tipoMovimiento;
            }

            // Buscar en otros campos posibles
            var camposMovimiento = new[] { "tipo_movimiento", "movimiento", "operacion", "poliza.operacion" };

            foreach (var campo in camposMovimiento)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    var valor = campos[campo].Trim().ToUpperInvariant();

                    // Mapear valores comunes
                    if (valor.Contains("EMISION") || valor.Contains("NUEVA") || valor.Contains("ALTA"))
                        return "EMISION";
                    if (valor.Contains("RENOVACION") || valor.Contains("RENOV"))
                        return "RENOVACION";
                    if (valor.Contains("MODIFICACION") || valor.Contains("MODIF"))
                        return "MODIFICACION";
                    if (valor.Contains("ANULACION") || valor.Contains("BAJA"))
                        return "ANULACION";

                    return valor;
                }
            }

            return "EMISION"; // Default cambiado a EMISION
        }

        private string MapearUsoADestino(string uso)
        {
            return uso?.ToUpperInvariant() switch
            {
                "PARTICULAR" => "PARTICULAR",
                "COMERCIAL" => "COMERCIAL",
                "TAXI" => "TAXI",
                "REMISE" => "REMISE",
                "CARGA" => "CARGA",
                _ => "PARTICULAR" // Default
            };
        }

        private string DeterminarCategoria(string tipoVehiculo, string uso)
        {
            // Lógica para determinar categoría basada en tipo y uso
            if (!string.IsNullOrEmpty(tipoVehiculo))
            {
                if (tipoVehiculo.Contains("AUTOMOVIL", StringComparison.OrdinalIgnoreCase))
                    return "AUTOMOVIL";
                if (tipoVehiculo.Contains("CAMIONETA", StringComparison.OrdinalIgnoreCase))
                    return "CAMIONETA";
                if (tipoVehiculo.Contains("MOTOCICLETA", StringComparison.OrdinalIgnoreCase))
                    return "MOTOCICLETA";
            }

            return "AUTOMOVIL"; // Default
        }

        private string DeterminarMoneda(Dictionary<string, string> campos)
        {
            // Buscar primero el campo específico de Azure Document Intelligence
            if (campos.ContainsKey("poliza.moneda") && !string.IsNullOrEmpty(campos["poliza.moneda"]))
            {
                var monedaAzure = campos["poliza.moneda"].ToUpperInvariant().Trim();
                _logger.LogDebug("✅ Moneda encontrada en Azure: {Moneda}", monedaAzure);

                // Mapear valores específicos de Azure
                if (monedaAzure.Contains("PESO") || monedaAzure.Contains("URUGUAYO") || monedaAzure.Contains("UYU"))
                    return "UYU";
                if (monedaAzure.Contains("DOLAR") || monedaAzure.Contains("USD") || monedaAzure.Contains("DOLARES"))
                    return "USD";
                if (monedaAzure.Contains("EURO") || monedaAzure.Contains("EUR"))
                    return "EUR";
            }

            // Buscar en otros campos posibles
            var camposMoneda = new[] { "moneda", "currency", "financiero.moneda", "divisa" };

            foreach (var campo in camposMoneda)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    var moneda = campos[campo].ToUpperInvariant();
                    if (moneda.Contains("USD") || moneda.Contains("DOLAR"))
                        return "USD";
                    if (moneda.Contains("UYU") || moneda.Contains("PESO"))
                        return "UYU";
                    if (moneda.Contains("EUR") || moneda.Contains("EURO"))
                        return "EUR";
                }
            }

            return "UYU"; // Default uruguayo
        }

        private int MapearCodigoMoneda(string moneda)
        {
            return moneda switch
            {
                "UYU" => 1,
                "USD" => 2,
                _ => 1
            };
        }

        private AzureInformacionCuotasDto ConvertirInformacionCuotas(InformacionCuotas cuotasLegacy)
        {
            return new AzureInformacionCuotasDto
            {
                CantidadTotal = cuotasLegacy.CantidadTotal,
                Cuotas = cuotasLegacy.Cuotas.Select(c => new AzureDetalleCuotaDto
                {
                    Numero = c.Numero,
                    FechaVencimiento = c.FechaVencimiento,
                    Monto = c.Monto,
                    Estado = c.Estado
                }).ToList()
            };
        }

        private List<AzureBonificacionDto> BuscarBonificaciones(Dictionary<string, string> campos)
        {
            var bonificaciones = new List<AzureBonificacionDto>();

            // Buscar patrones de bonificaciones
            foreach (var campo in campos)
            {
                if (campo.Key.Contains("bonif", StringComparison.OrdinalIgnoreCase) ||
                    campo.Value.Contains("bonificacion", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(campo.Value, @"(\d+(?:\.\d+)?)%?", RegexOptions.IgnoreCase);
                    if (match.Success && decimal.TryParse(match.Groups[1].Value, out decimal valor))
                    {
                        bonificaciones.Add(new AzureBonificacionDto
                        {
                            Tipo = campo.Key,
                            Porcentaje = valor,
                            Descripcion = campo.Value
                        });
                    }
                }
            }

            return bonificaciones;
        }

        private string BuscarObservaciones(Dictionary<string, string> campos)
        {
            var camposObservaciones = new[] { "observaciones", "notas", "comentarios", "observacion" };

            foreach (var campo in camposObservaciones)
            {
                if (campos.ContainsKey(campo) && !string.IsNullOrEmpty(campos[campo]))
                {
                    return campos[campo];
                }
            }

            return "";
        }

        #endregion
    }
}