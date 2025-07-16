using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI;
using System.ComponentModel.DataAnnotations;
using static ClienteMatchResult;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PolizaProcessingController : ControllerBase
    {
        private readonly IDocumentExtractionService _documentService;
        private readonly IClienteMatchingService _clienteMatchingService;
        private readonly ILogger<PolizaProcessingController> _logger;

        public PolizaProcessingController(
            IDocumentExtractionService documentService,
            IClienteMatchingService clienteMatchingService,
            ILogger<PolizaProcessingController> logger)
        {
            _documentService = documentService;
            _clienteMatchingService = clienteMatchingService;
            _logger = logger;
        }

        /// <summary>
        /// PASO 1: Procesar documento y extraer datos
        /// </summary>
        [HttpPost("extract-document")]
        [ProducesResponseType(typeof(DocumentExtractResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentExtractResult>> ExtractDocument([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🔄 PASO 1: Extrayendo datos de documento {FileName}", file.FileName);

                var result = await _documentService.ProcessDocumentAsync(file);

                _logger.LogInformation("✅ PASO 1 COMPLETADO: Datos extraídos de {FileName}", file.FileName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error extrayendo datos del documento");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// PASO 2: Buscar clientes con los datos extraídos
        /// </summary>
        [HttpPost("search-clients")]
        [ProducesResponseType(typeof(ClienteMatchResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClienteMatchResult>> SearchClients([FromBody] DatosClienteExtraidos datosCliente)
        {
            try
            {
                if (datosCliente == null)
                {
                    return BadRequest(new { error = "Datos del cliente requeridos" });
                }

                _logger.LogInformation("🔄 PASO 2: Buscando clientes para '{Nombre}' - '{Documento}'",
                    datosCliente.Nombre, datosCliente.Documento);

                var result = await _clienteMatchingService.BuscarClienteAsync(datosCliente);

                _logger.LogInformation("✅ PASO 2 COMPLETADO: {TipoResultado} - {Count} matches encontrados",
                    result.TipoResultado, result.Matches.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando clientes");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// PASO 3: Crear póliza con cliente seleccionado
        /// </summary>
        [HttpPost("create-poliza")]
        [ProducesResponseType(typeof(CrearPolizaResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CrearPolizaResponse>> CreatePoliza([FromBody] CrearPolizaConClienteRequest request)
        {
            try
            {
                if (request == null || request.ClienteId <= 0 || request.DatosPoliza == null)
                {
                    return BadRequest(new { error = "Datos de solicitud inválidos" });
                }

                _logger.LogInformation("🔄 PASO 3: Creando póliza {NumeroPoliza} para cliente {ClienteId}",
                    request.DatosPoliza.NumeroPoliza, request.ClienteId);

                var result = await _documentService.CrearPolizaConClienteAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("✅ PASO 3 COMPLETADO: Póliza {NumeroPoliza} creada exitosamente",
                        request.DatosPoliza.NumeroPoliza);
                }
                else
                {
                    _logger.LogWarning("⚠️ PASO 3 FALLIDO: {Message}", result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creando póliza");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// FLUJO SIMPLIFICADO: Solo extracción y búsqueda
        /// </summary>
        [HttpPost("process-basic")]
        [ProducesResponseType(typeof(ProcessBasicResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProcessBasicResponse>> ProcessBasic([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🔄 FLUJO BÁSICO: Procesando {FileName}", file.FileName);

                var response = new ProcessBasicResponse
                {
                    NombreArchivo = file.FileName,
                    FechaInicio = DateTime.Now
                };

                // PASO 1: Extraer datos
                var extractResult = await _documentService.ProcessDocumentAsync(file);
                response.ExtraccionCompletada = true;
                response.DatosExtraidos = extractResult;

                if (extractResult.EstadoProcesamiento.Contains("ERROR"))
                {
                    response.Success = false;
                    response.Message = "Error extrayendo datos del documento";
                    return Ok(response);
                }

                // PASO 2: Buscar cliente automáticamente
                var clienteMatch = await _clienteMatchingService.BuscarClienteAsync(extractResult.DatosClienteBusqueda);
                response.BusquedaCompletada = true;
                response.ResultadoBusqueda = clienteMatch;

                // Determinar siguiente paso
                switch (clienteMatch.TipoResultado)
                {
                    case TipoResultadoCliente.MatchExacto:
                        response.Success = true;
                        response.Message = $"Cliente encontrado automáticamente: {clienteMatch.Matches[0].Cliente.Clinom}";
                        response.RequiereIntervencion = false;
                        response.SiguientePaso = "crear_poliza_automatico";
                        break;

                    case TipoResultadoCliente.MatchMuyProbable:
                        response.Success = true;
                        response.Message = $"Cliente probable: {clienteMatch.Matches[0].Cliente.Clinom}. Requiere confirmación.";
                        response.RequiereIntervencion = true;
                        response.SiguientePaso = "confirmar_cliente";
                        break;

                    case TipoResultadoCliente.MultiplesMatches:
                        response.Success = true;
                        response.Message = $"Se encontraron {clienteMatch.Matches.Count} clientes similares.";
                        response.RequiereIntervencion = true;
                        response.SiguientePaso = "seleccionar_cliente";
                        break;

                    default:
                        response.Success = true;
                        response.Message = "No se encontraron coincidencias automáticas.";
                        response.RequiereIntervencion = true;
                        response.SiguientePaso = "busqueda_manual";
                        break;
                }

                response.FechaFin = DateTime.Now;
                response.TiempoTotal = (response.FechaFin - response.FechaInicio).TotalMilliseconds;

                _logger.LogInformation("✅ FLUJO BÁSICO COMPLETADO: {FileName} - {Message}",
                    file.FileName, response.Message);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en flujo básico");

                return Ok(new ProcessBasicResponse
                {
                    NombreArchivo = file.FileName,
                    Success = false,
                    Message = $"Error procesando documento: {ex.Message}",
                    RequiereIntervencion = true,
                    FechaInicio = DateTime.Now,
                    FechaFin = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Debug - ver datos extraídos en detalle
        /// </summary>
        [HttpPost("debug-extraction")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DebugExtraction([Required] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No se ha proporcionado un archivo válido" });
                }

                _logger.LogInformation("🔧 DEBUG: Analizando extracción de {FileName}", file.FileName);

                var extractResult = await _documentService.ProcessDocumentAsync(file);

                return Ok(new
                {
                    archivo = file.FileName,
                    timestamp = DateTime.Now,
                    extraccion = extractResult,
                    resumen = new
                    {
                        numeroPoliza = extractResult.DatosPoliza.NumeroPoliza,
                        cliente = extractResult.DatosClienteBusqueda.Nombre,
                        documento = extractResult.DatosClienteBusqueda.Documento,
                        vehiculo = extractResult.DatosPoliza.DescripcionVehiculo,
                        primaComercial = extractResult.DatosPoliza.PrimaComercial,
                        premioTotal = extractResult.DatosPoliza.PremioTotal,
                        vigenciaDesde = extractResult.DatosPoliza.VigenciaDesde,
                        vigenciaHasta = extractResult.DatosPoliza.VigenciaHasta,
                        advertencias = extractResult.Advertencias.Count,
                        confianza = extractResult.ConfianzaExtraccion
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en debug de extracción");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    

        /// <summary>
        /// CONFIRMAR CLIENTE - Confirmar cliente seleccionado y preparar datos para Velneo
        /// </summary>
        [HttpPost("confirm-client-for-poliza")]
            [ProducesResponseType(typeof(object), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(500)]
            public async Task<ActionResult> ConfirmClientForPoliza([FromBody] ConfirmClientRequest request)
            {
                try
                {
                    if (request == null)
                    {
                        return BadRequest(new { error = "Request requerido" });
                    }
        
                    if (request.ClienteId <= 0)
                    {
                        return BadRequest(new { error = "ID de cliente válido requerido" });
                    }
        
                    if (request.DatosPoliza == null)
                    {
                        return BadRequest(new { error = "Datos de póliza requeridos" });
                    }
        
                    _logger.LogInformation("✅ CONFIRMANDO CLIENTE: {ClienteId} para póliza {NumeroPoliza}",
                        request.ClienteId, request.DatosPoliza.NumeroPoliza);
        
                    // Obtener datos completos del cliente confirmado
                    var velneoService = HttpContext.RequestServices.GetRequiredService<IVelneoApiService>();
                    var cliente = await velneoService.GetClienteAsync(request.ClienteId);
        
                    if (cliente == null)
                    {
                        return BadRequest(new { error = $"Cliente {request.ClienteId} no encontrado" });
                    }
        
                    // Preparar datos completos para envío a Velneo
                    var polizaParaVelneo = new
                    {
                        // Datos del cliente confirmado
                        cliente = new
                        {
                            id = cliente.Clinro,
                            nombre = cliente.Clinom,
                            documento = cliente.Cliruc,
                            direccion = cliente.Clidir,
                            telefono = cliente.Clitelcel,
                            email = cliente.Cliemail,
                            localidad = cliente.Clilocnom,
                            activo = cliente.Activo
                        },
        
                        // Datos de la póliza extraídos del documento
                        poliza = new
                        {
                            numeroPoliza = request.DatosPoliza.NumeroPoliza,
                            asegurado = request.DatosPoliza.Asegurado,
                            vehiculo = request.DatosPoliza.Vehiculo,
                            marca = request.DatosPoliza.Marca,
                            modelo = request.DatosPoliza.Modelo,
                            motor = request.DatosPoliza.Motor,
                            chasis = request.DatosPoliza.Chasis,
                            primaComercial = request.DatosPoliza.PrimaComercial,
                            premioTotal = request.DatosPoliza.PremioTotal,
                            vigenciaDesde = request.DatosPoliza.VigenciaDesde,
                            vigenciaHasta = request.DatosPoliza.VigenciaHasta,
                            corredor = request.DatosPoliza.Corredor,
                            plan = request.DatosPoliza.Plan,
                            ramo = request.DatosPoliza.Ramo
                        },
        
                        // Metadatos del proceso
                        confirmacion = new
                        {
                            fechaConfirmacion = DateTime.UtcNow,
                            clienteSeleccionadoManualmente = request.SeleccionManual,
                            observaciones = request.Observaciones ?? "",
                            usuarioConfirmacion = User?.Identity?.Name ?? "Sistema"
                        }
                    };
        
                    _logger.LogInformation("✅ CLIENTE CONFIRMADO: {ClienteNombre} para póliza {NumeroPoliza}",
                        cliente.Clinom, request.DatosPoliza.NumeroPoliza);
        
                    return Ok(new
                    {
                        estado = "CLIENTE_CONFIRMADO",
                        mensaje = $"Cliente {cliente.Clinom} confirmado para póliza {request.DatosPoliza.NumeroPoliza}",
                        datosCompletos = polizaParaVelneo,
                        siguientePaso = "enviar_a_velneo",
                        listoParaVelneo = true,
                        resumen = new
                        {
                            cliente = $"{cliente.Clinom} ({cliente.Cliruc})",
                            poliza = request.DatosPoliza.NumeroPoliza,
                            vehiculo = request.DatosPoliza.Vehiculo,
                            prima = request.DatosPoliza.PrimaComercial,
                            procesoCompleto = true
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error confirmando cliente para póliza");
                    return StatusCode(500, new { error = ex.Message });
                }
            }
         }
    }


    public class ConfirmClientRequest
    {
        public int ClienteId { get; set; }
        public SmartExtractedData DatosPoliza { get; set; } = new();
        public bool SeleccionManual { get; set; } = true;
        public string? Observaciones { get; set; }
    }

    // ===== MODELO DE RESPONSE SIMPLIFICADO =====

    public class ProcessBasicResponse
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool RequiereIntervencion { get; set; }
        public string SiguientePaso { get; set; } = string.Empty;

        public bool ExtraccionCompletada { get; set; }
        public bool BusquedaCompletada { get; set; }

        public DocumentExtractResult? DatosExtraidos { get; set; }
        public ClienteMatchResult? ResultadoBusqueda { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public double TiempoTotal { get; set; }

        public string EstadoGeneral => Success
            ? (RequiereIntervencion ? "Requiere intervención manual" : "Listo para automatizar")
            : "Error en procesamiento";
    }
