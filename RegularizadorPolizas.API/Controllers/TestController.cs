using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External;
using Microsoft.Extensions.Logging;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<DocumentResultParser> _logger;

        public TestController(ILogger<DocumentResultParser> logger)
        {
            _logger = logger;
        }

        [HttpPost("document-parser-basic")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public ActionResult TestDocumentParserBasic()
        {
            try
            {
                var documentoPrueba = new DocumentResultDto
                {
                    DocumentoId = 999,
                    NombreArchivo = "test_poliza_bse.pdf",
                    EstadoProcesamiento = "PROCESADO",
                    ConfianzaExtraccion = 89.5m,
                    RequiereRevision = false,
                    CamposExtraidos = new Dictionary<string, string>
                    {
                        // Póliza básica
                        ["numero_poliza"] = "BSE-AUTO-2024-123456",
                        ["ramo"] = "AUTOMOTOR",
                        ["endoso"] = "001",

                        // Cliente
                        ["cliente_nombre"] = "María Elena García Rodríguez",
                        ["cliente_documento"] = "1.234.567-8",
                        ["cliente_email"] = "maria.garcia@gmail.com",
                        ["cliente_telefono"] = "099-876-543",
                        ["cliente_direccion"] = "18 de Julio 1234 Apto 505",
                        ["localidad"] = "Montevideo",
                        ["departamento"] = "Montevideo",
                        ["codigo_postal"] = "11200",

                        // Vehículo
                        ["vehiculo_marca"] = "Toyota",
                        ["vehiculo_matricula"] = "SBD-5678",
                        ["vehiculo_año"] = "2022",
                        ["vehiculo_motor"] = "4A91T123456",
                        ["vehiculo_padron"] = "1234567890",
                        ["vehiculo_chasis"] = "JTDBT4K3XAJ123456",

                        // Financiero
                        ["premio_total"] = "28750.50",
                        ["impuestos"] = "2875.05",
                        ["total_impuestos"] = "31625.55",
                        ["cuotas"] = "6",
                        ["moneda"] = "UYU",

                        // Fechas
                        ["fecha_desde"] = "15/04/2024",
                        ["fecha_hasta"] = "15/04/2025",

                        // Cobertura
                        ["tipo_cobertura"] = "Todo Riesgo",
                        ["corredor"] = "Estudio ABC Seguros"
                    }
                };

                var parser = new DocumentResultParser(_logger);
                var polizaResultado = parser.ParseToPolizaDto(documentoPrueba);

                var resultado = new
                {
                    success = true,
                    message = "✅ DocumentParser funcionando correctamente",
                    documentoOriginal = new
                    {
                        id = documentoPrueba.DocumentoId,
                        archivo = documentoPrueba.NombreArchivo,
                        confianza = documentoPrueba.ConfianzaExtraccion,
                        totalCampos = documentoPrueba.CamposExtraidos.Count
                    },
                    polizaGenerada = new
                    {
                        // Información básica
                        numeroPoliza = polizaResultado.Conpol,
                        ramo = polizaResultado.Ramo,
                        endoso = polizaResultado.Conend,
                        activa = polizaResultado.Activo,
                        procesada = polizaResultado.Procesado,

                        // Cliente
                        clienteNombre = polizaResultado.Clinom,
                        clienteDocumento = polizaResultado.Cliruc,
                        clienteEmail = polizaResultado.Cliemail,
                        clienteTelefono = polizaResultado.Clitelcel,
                        clienteDireccion = polizaResultado.Condom,
                        localidad = polizaResultado.Clilocnom,
                        departamento = polizaResultado.Clidptnom,

                        // Vehículo
                        vehiculoMarca = polizaResultado.Conmaraut,
                        vehiculoMatricula = polizaResultado.Conmataut,
                        vehiculoAño = polizaResultado.Conanioaut,
                        vehiculoMotor = polizaResultado.Conmotor,
                        vehiculoPadron = polizaResultado.Conpadaut,
                        vehiculoChasis = polizaResultado.Conchasis,

                        // Financiero
                        premio = polizaResultado.Conpremio,
                        total = polizaResultado.Contot,
                        impuestos = polizaResultado.Conimp,
                        cuotas = polizaResultado.Concuo,
                        moneda = polizaResultado.Moncod,

                        // Fechas
                        fechaDesde = polizaResultado.Confchdes,
                        fechaHasta = polizaResultado.Confchhas,

                        // Otros
                        cobertura = polizaResultado.Contpocob,
                        observaciones = polizaResultado.Observaciones
                    },
                    validaciones = new
                    {
                        tieneNumeroPoliza = !string.IsNullOrEmpty(polizaResultado.Conpol),
                        tieneCliente = !string.IsNullOrEmpty(polizaResultado.Clinom),
                        tieneVehiculo = !string.IsNullOrEmpty(polizaResultado.Conmaraut),
                        tieneMonto = polizaResultado.Conpremio.HasValue && polizaResultado.Conpremio > 0,
                        tieneFechas = polizaResultado.Confchdes.HasValue && polizaResultado.Confchhas.HasValue,
                        fechasValidas = ValidarFechas(polizaResultado),
                        documentoLimpio = !string.IsNullOrEmpty(polizaResultado.Cliruc) &&
                                         polizaResultado.Cliruc == "12345678", // Sin puntos ni guiones
                        matriculaLimpia = !string.IsNullOrEmpty(polizaResultado.Conmataut) &&
                                         polizaResultado.Conmataut == "SBD5678" // Sin guión
                    }
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = "❌ Error en DocumentParser",
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("document-parser-custom")]
        [ProducesResponseType(typeof(object), 200)]
        public ActionResult TestDocumentParserCustom([FromBody] Dictionary<string, string> camposPersonalizados)
        {
            try
            {
                if (camposPersonalizados == null || !camposPersonalizados.Any())
                {
                    return BadRequest(new
                    {
                        error = "Envía un JSON con campos para probar",
                        ejemplo = new Dictionary<string, string>
                        {
                            ["numero_poliza"] = "TEST-001",
                            ["cliente_nombre"] = "Juan Pérez",
                            ["vehiculo_marca"] = "Toyota",
                            ["premio_total"] = "15000"
                        }
                    });
                }

                var documento = new DocumentResultDto
                {
                    DocumentoId = 998,
                    NombreArchivo = "test_custom.pdf",
                    EstadoProcesamiento = "PROCESADO",
                    CamposExtraidos = camposPersonalizados
                };

                var parser = new DocumentResultParser(_logger);
                var resultado = parser.ParseToPolizaDto(documento);

                return Ok(new
                {
                    success = true,
                    message = "✅ Test personalizado completado",
                    camposEntrada = camposPersonalizados,
                    polizaGenerada = resultado,
                    estadisticas = new
                    {
                        camposEnviados = camposPersonalizados.Count,
                        camposMapeados = ContarCamposMapeados(resultado),
                        camposVacios = ContarCamposVacios(resultado),
                        porcentajeMapeado = CalcularPorcentajeMapeado(camposPersonalizados.Count, resultado)
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("test-date-formats")]
        [ProducesResponseType(typeof(object), 200)]
        public ActionResult TestDateFormats()
        {
            var formatosFecha = new[]
            {
                "15/03/2024",
                "2024-03-15",
                "15-03-2024",
                "3/15/2024",
                "2024/03/15",
                "15 de marzo de 2024"
            };

            var resultados = new List<object>();

            foreach (var formato in formatosFecha)
            {
                try
                {
                    var documento = new DocumentResultDto
                    {
                        CamposExtraidos = new Dictionary<string, string>
                        {
                            ["numero_poliza"] = $"TEST-FECHA-{formatosFecha.ToList().IndexOf(formato)}",
                            ["fecha_desde"] = formato,
                            ["fecha_hasta"] = formato // Usamos la misma para ver si corrige automáticamente
                        }
                    };

                    var parser = new DocumentResultParser(_logger);
                    var poliza = parser.ParseToPolizaDto(documento);

                    resultados.Add(new
                    {
                        formatoOriginal = formato,
                        fechaParseda = poliza.Confchdes,
                        fechaCorregida = poliza.Confchhas,
                        exitoso = poliza.Confchdes.HasValue,
                        observaciones = poliza.Observaciones
                    });
                }
                catch (Exception ex)
                {
                    resultados.Add(new
                    {
                        formatoOriginal = formato,
                        error = ex.Message,
                        exitoso = false
                    });
                }
            }

            return Ok(new
            {
                message = "Test de formatos de fecha completado",
                resultados = resultados,
                exitosos = resultados.Count(r => (bool)r.GetType().GetProperty("exitoso")?.GetValue(r)),
                total = resultados.Count
            });
        }

        [HttpPost("test-data-cleaning")]
        [ProducesResponseType(typeof(object), 200)]
        public ActionResult TestDataCleaning()
        {
            var documento = new DocumentResultDto
            {
                CamposExtraidos = new Dictionary<string, string>
                {
                    ["numero_poliza"] = "  BSE-AUTO-123  \n\r\t",
                    ["cliente_documento"] = "1.234.567-8",
                    ["vehiculo_matricula"] = "SBD-1234",
                    ["cliente_telefono"] = "099-123-456",
                    ["premio_total"] = "$UYU 15,000.50",
                    ["cliente_email"] = "test@email.com",
                    ["cliente_email_invalido"] = "email-sin-arroba"
                }
            };

            var parser = new DocumentResultParser(_logger);
            var poliza = parser.ParseToPolizaDto(documento);

            return Ok(new
            {
                message = "Test de limpieza de datos",
                original = documento.CamposExtraidos,
                limpiado = new
                {
                    numeroPoliza = poliza.Conpol, 
                    documento = poliza.Cliruc, 
                    matricula = poliza.Conmataut, 
                    telefono = poliza.Clitelcel,
                    premio = poliza.Conpremio, 
                    email = poliza.Cliemail 
                },
                validaciones = new
                {
                    numeroLimpio = poliza.Conpol == "BSE-AUTO-123",
                    documentoLimpio = poliza.Cliruc == "12345678",
                    matriculaLimpia = poliza.Conmataut == "SBD1234",
                    telefonoLimpio = poliza.Clitelcel == "099123456",
                    premioParseado = poliza.Conpremio == 15000.50m,
                    emailValido = poliza.Cliemail == "test@email.com"
                }
            });
        }

        #region Métodos Helper

        private bool ValidarFechas(PolizaDto poliza)
        {
            if (!poliza.Confchdes.HasValue || !poliza.Confchhas.HasValue)
                return false;

            return poliza.Confchdes < poliza.Confchhas;
        }

        private int ContarCamposMapeados(PolizaDto poliza)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(poliza.Conpol)) count++;
            if (!string.IsNullOrEmpty(poliza.Clinom)) count++;
            if (!string.IsNullOrEmpty(poliza.Cliruc)) count++;
            if (!string.IsNullOrEmpty(poliza.Conmaraut)) count++;
            if (!string.IsNullOrEmpty(poliza.Conmataut)) count++;
            if (poliza.Conanioaut.HasValue) count++;
            if (poliza.Confchdes.HasValue) count++;
            if (poliza.Confchhas.HasValue) count++;
            if (poliza.Conpremio.HasValue) count++;
            if (poliza.Moncod.HasValue) count++;
            return count;
        }

        private int ContarCamposVacios(PolizaDto poliza)
        {
            int count = 0;
            if (string.IsNullOrEmpty(poliza.Conpol)) count++;
            if (string.IsNullOrEmpty(poliza.Clinom)) count++;
            if (string.IsNullOrEmpty(poliza.Conmaraut)) count++;
            if (string.IsNullOrEmpty(poliza.Conmataut)) count++;
            if (!poliza.Confchdes.HasValue) count++;
            if (!poliza.Confchhas.HasValue) count++;
            if (!poliza.Conpremio.HasValue) count++;
            return count;
        }

        private double CalcularPorcentajeMapeado(int camposEnviados, PolizaDto poliza)
        {
            if (camposEnviados == 0) return 0;

            int camposMapeados = ContarCamposMapeados(poliza);
            return Math.Round((double)camposMapeados / Math.Min(camposEnviados, 10) * 100, 1);
        }

        #endregion
    }
}