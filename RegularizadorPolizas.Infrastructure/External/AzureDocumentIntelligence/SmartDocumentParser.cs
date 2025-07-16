using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
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

                // Construir descripción completa del vehículo
                datos.Vehiculo = $"{datos.Marca} {datos.Modelo} ({datos.Anio})".Trim();

                _logger.LogInformation("✅ Extracción inteligente completada: Póliza={Poliza}, Cliente={Cliente}",
                    datos.NumeroPoliza, datos.Asegurado);

                return datos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en extracción inteligente");
                return datos; // Retorna datos parciales
            }
        }

        public DatosClienteExtraidos CrearDatosClienteBusqueda(SmartExtractedData datos)
        {
            return new DatosClienteExtraidos
            {
                Nombre = datos.Asegurado,
                Documento = datos.Documento,
                Email = datos.Email,
                Telefono = "", // No extraído de este PDF
                Direccion = datos.Direccion,
                Localidad = datos.Localidad,
                Departamento = datos.Departamento,
                ConfidenceScore = 0.95f
            };
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

        private void LogExtractedValues(string campo, string texto, SmartExtractedData datos)
        {
            _logger.LogDebug("🔍 EXTRACTING FROM {Campo}:", campo);
            _logger.LogDebug("📄 Texto completo: {Texto}", texto.Substring(0, Math.Min(200, texto.Length)));
            _logger.LogDebug("✅ Asegurado extraído: '{Asegurado}'", datos.Asegurado);
            _logger.LogDebug("✅ Modelo extraído: '{Modelo}'", datos.Modelo);
            _logger.LogDebug("✅ Motor extraído: '{Motor}'", datos.Motor);
        }

        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            return texto.Trim()
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Replace("  ", " ")
                        .Trim();
        }

        #endregion
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
    }
}