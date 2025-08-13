using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Azure;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;
using RegularizadorPolizas.Application.Mappers;

namespace RegularizadorPolizas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PolizaController : ControllerBase
    {
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly IAzureToVelneoMapper _azureMapper;
        private readonly ITenantService _tenantService;
        private readonly ILogger<PolizaController> _logger;

        public PolizaController(
            IVelneoMaestrosService velneoMaestrosService,
            IAzureToVelneoMapper azureMapper,
            ITenantService tenantService,
            ILogger<PolizaController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService;
            _azureMapper = azureMapper;
            _tenantService = tenantService;
            _logger = logger;
        }

        #region CRUD ENDPOINTS

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PolizaDto>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<PolizaDto>>> GetAllPolizas()
        {
            try
            {
                _logger.LogInformation("🔍 Obteniendo todas las pólizas desde VelneoMaestrosService");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var polizas = await _velneoMaestrosService.GetPolizasAsync();

                _logger.LogInformation("✅ {Count} pólizas obtenidas exitosamente", polizas.Count());

                return Ok(new
                {
                    success = true,
                    data = polizas,
                    total = polizas.Count(),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service",
                    message = $"Se obtuvieron {polizas.Count()} pólizas exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo todas las pólizas");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor obteniendo pólizas",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// ✅ CORREGIDO: Obtener pólizas por cliente usando VelneoMaestrosService
        /// </summary>
        [HttpGet("client/{clienteId}")]
        [ProducesResponseType(typeof(IEnumerable<PolizaDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult<IEnumerable<PolizaDto>>> GetPolizasByClient(int clienteId)
        {
            try
            {
                _logger.LogInformation("🔍 Obteniendo pólizas para cliente {ClienteId}", clienteId);

                if (clienteId <= 0)
                {
                    _logger.LogWarning("🚫 ClienteId inválido: {ClienteId}", clienteId);
                    return BadRequest(new
                    {
                        success = false,
                        message = "El ID del cliente debe ser mayor a 0",
                        clienteId = clienteId
                    });
                }

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var polizas = await _velneoMaestrosService.GetPolizasByClientAsync(clienteId);

                if (!polizas.Any())
                {
                    _logger.LogInformation("📋 No se encontraron pólizas para cliente {ClienteId}", clienteId);
                    return Ok(new
                    {
                        success = true,
                        data = new List<PolizaDto>(),
                        total = 0,
                        clienteId = clienteId,
                        message = $"No se encontraron pólizas para el cliente {clienteId}",
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogInformation("✅ {Count} pólizas encontradas para cliente {ClienteId}",
                    polizas.Count(), clienteId);

                return Ok(new
                {
                    success = true,
                    data = polizas,
                    total = polizas.Count(),
                    clienteId = clienteId,
                    message = $"Se encontraron {polizas.Count()} pólizas para el cliente {clienteId}",
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo pólizas para cliente {ClienteId}", clienteId);

                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error interno del servidor obteniendo pólizas para cliente {clienteId}",
                    error = ex.Message,
                    clienteId = clienteId,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// ✅ CORREGIDO: Obtener póliza por ID usando VelneoMaestrosService
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetPolizaById(int id)
        {
            try
            {
                _logger.LogInformation("🔍 Buscando póliza: {PolizaId}", id);

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var poliza = await _velneoMaestrosService.GetPolizaAsync(id);

                if (poliza == null)
                {
                    _logger.LogWarning("🚫 Póliza no encontrada: {PolizaId}", id);
                    return NotFound(new { message = $"Póliza {id} no encontrada" });
                }

                _logger.LogInformation("✅ Póliza {PolizaId} encontrada: {NumeroPoliza}", id, poliza.Conpol);

                return Ok(new
                {
                    success = true,
                    data = poliza,
                    polizaId = id,
                    numeroPoliza = poliza.Conpol,
                    source = "velneo_maestros_service",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("🚫 Póliza no encontrada: {PolizaId}", id);
                return NotFound(new { message = $"Póliza {id} no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al buscar póliza {PolizaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// ✅ CORREGIDO: Crear nueva póliza usando VelneoMaestrosService
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult> CreatePoliza([FromBody] PolizaCreateRequest request)
        {
            try
            {
                // ✅ VALIDACIÓN INICIAL
                if (request == null)
                {
                    _logger.LogWarning("🚫 Request nulo recibido para crear póliza");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos de póliza son requeridos"
                    });
                }

                _logger.LogInformation("🚀 INICIANDO CREACIÓN DE PÓLIZA: {NumeroPoliza} para cliente {ClienteId}",
                    request.Conpol, request.Clinro);

                // ✅ VALIDAR MODELO CON DATA ANNOTATIONS
                if (!ModelState.IsValid)
                {
                    var errores = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("🚫 Validación del modelo fallida: {Errores}", string.Join(", ", errores));

                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos de póliza inválidos",
                        errores = errores,
                        numeroPoliza = request.Conpol
                    });
                }

                // ✅ LOG DE DATOS RECIBIDOS PARA DEBUG
                LogDatosRecibidos(request);

                // ✅ PROCESAR CAMPOS ANTES DEL ENVÍO
                await ProcesarCamposAdicionales(request);

                // ✅ CORREGIDO: ENVIAR A VELNEO CON VelneoMaestrosService
                var resultado = await _velneoMaestrosService.CreatePolizaFromRequestAsync(request);

                if (resultado != null)
                {
                    _logger.LogInformation("✅ PÓLIZA CREADA EXITOSAMENTE: {NumeroPoliza}", request.Conpol);

                    return CreatedAtAction(
                        nameof(GetPolizaById),
                        new { id = request.Conpol },
                        new
                        {
                            success = true,
                            message = "Póliza creada exitosamente en Velneo",
                            numeroPoliza = request.Conpol,
                            clienteId = request.Clinro,
                            companiaId = request.Comcod,
                            seccionId = ResolverSeccionParaRespuesta(request),
                            datosEnviados = GenerarResumenDatos(request),
                            polizaCreada = resultado,
                            timestamp = DateTime.UtcNow,
                            procesadoConIA = request.ProcesadoConIA,
                            source = "velneo_maestros_service"
                        });
                }
                else
                {
                    _logger.LogError("❌ Velneo retornó respuesta vacía para póliza {NumeroPoliza}", request.Conpol);

                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Velneo retornó respuesta vacía",
                        numeroPoliza = request.Conpol,
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "❌ Error de validación al crear póliza {NumeroPoliza}", request?.Conpol);

                return BadRequest(new
                {
                    success = false,
                    message = "Error de validación",
                    error = ex.Message,
                    numeroPoliza = request?.Conpol,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "❌ Error de operación al crear póliza {NumeroPoliza}", request?.Conpol);

                return StatusCode(422, new
                {
                    success = false,
                    message = "Error en la operación con Velneo",
                    error = ex.Message,
                    numeroPoliza = request?.Conpol,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error interno al crear póliza {NumeroPoliza}", request?.Conpol);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message,
                    numeroPoliza = request?.Conpol,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// ✅ CORREGIDO: Estadísticas usando VelneoMaestrosService
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult> GetPolizasStats()
        {
            try
            {
                _logger.LogInformation("📊 Obteniendo estadísticas de pólizas");

                // ✅ CORREGIDO: usar VelneoMaestrosService
                var polizas = await _velneoMaestrosService.GetPolizasAsync();

                var stats = new
                {
                    totalPolizas = polizas.Count(),
                    porCompania = polizas.GroupBy(p => p.Comcod)
                        .Select(g => new { companiaId = g.Key, total = g.Count() })
                        .OrderByDescending(x => x.total)
                        .Take(10),
                    porSeccion = polizas.GroupBy(p => p.Seccod)
                        .Select(g => new { seccionId = g.Key, total = g.Count() })
                        .OrderByDescending(x => x.total)
                        .Take(10),
                    porEstado = polizas.GroupBy(p => p.Convig)
                        .Select(g => new { estado = g.Key, total = g.Count() }),
                    ultimasCreadas = polizas
                        .Where(p => !string.IsNullOrEmpty(p.Ingresado))
                        .OrderByDescending(p => p.Ingresado)
                        .Take(5)
                        .Select(p => new { numeroPoliza = p.Conpol, clienteId = p.Clinro, fechaIngreso = p.Ingresado }),
                    timestamp = DateTime.UtcNow,
                    source = "velneo_maestros_service"
                };

                _logger.LogInformation("✅ Estadísticas generadas: {Total} pólizas procesadas", stats.totalPolizas);

                return Ok(new
                {
                    success = true,
                    data = stats,
                    message = "Estadísticas de pólizas generadas exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error generando estadísticas de pólizas");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor generando estadísticas",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpPost("debug")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> DebugCreatePoliza([FromBody] PolizaCreateRequest request)
        {
            try
            {
                _logger.LogInformation("🧪 DEBUG: Iniciando debug de creación de póliza");

                // Verificar configuración del tenant
                var tenantId = _tenantService?.GetCurrentTenantId();
                var config = await _tenantService.GetCurrentTenantConfigurationAsync();

                _logger.LogInformation("🔧 Configuración del Tenant:");
                _logger.LogInformation("   TenantId: {TenantId}", tenantId);
                _logger.LogInformation("   Mode: {Mode}", config.Mode);
                _logger.LogInformation("   BaseUrl: {BaseUrl}", config.BaseUrl);
                _logger.LogInformation("   Active: {Active}", config.Activo);

                // Verificar conectividad básica
                _logger.LogInformation("🌐 Probando conectividad...");
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var testUrl = $"{config.BaseUrl?.TrimEnd('/')}/v1/clientes";
                _logger.LogInformation("   Test URL: {TestUrl}", testUrl);

                try
                {
                    var testResponse = await httpClient.GetAsync(testUrl);
                    _logger.LogInformation("   Conectividad: {Status}", testResponse.StatusCode);
                }
                catch (Exception connEx)
                {
                    _logger.LogError(connEx, "❌ Error de conectividad");
                }

                // Log del request recibido
                _logger.LogInformation("📋 Request recibido:");
                _logger.LogInformation("   Póliza: {NumeroPoliza}", request.Conpol);
                _logger.LogInformation("   Cliente: {ClienteId} - {ClienteNombre}", request.Clinro, request.Asegurado);
                _logger.LogInformation("   Compañía: {CompaniaId}", request.Comcod);
                _logger.LogInformation("   Premio: {Premio}", request.Conpremio);
                _logger.LogInformation("   Vigencia: {Desde} a {Hasta}", request.Confchdes, request.Confchhas);

                // Intentar crear la póliza con logging detallado
                _logger.LogInformation("🚀 Ejecutando CreatePolizaFromRequestAsync...");
                var resultado = await _velneoMaestrosService.CreatePolizaFromRequestAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Debug completado exitosamente",
                    tenantConfig = new
                    {
                        tenantId = tenantId,
                        mode = config.Mode,
                        baseUrl = config.BaseUrl,
                        active = config.Activo
                    },
                    request = new
                    {
                        numeroPoliza = request.Conpol,
                        clienteId = request.Clinro,
                        companiaId = request.Comcod,
                        premio = request.Conpremio
                    },
                    resultado = resultado,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en debug de póliza");

                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("debug/config")]
        public async Task<ActionResult> DebugConfig()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var config = await _tenantService.GetCurrentTenantConfigurationAsync();

                return Ok(new
                {
                    tenantId = tenantId,
                    mode = config.Mode,
                    baseUrl = config.BaseUrl,
                    active = config.Activo,
                    timeoutSeconds = config.TimeoutSeconds,
                    apiVersion = config.ApiVersion,
                    environment = config.Environment,
                    enableLogging = config.EnableLogging,
                    enableRetries = config.EnableRetries
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        #endregion

        #region AZURE DOCUMENT INTELLIGENCE ENDPOINTS

        /// <summary>
        /// ✅ NUEVO: Procesar resultado del escaneo de Azure Document Intelligence
        /// Este es el endpoint principal que el frontend llama después de escanear
        /// </summary>
        [HttpPost("procesar-escaneo")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<ActionResult> ProcesarEscaneoAzure([FromBody] AzureProcessResponseDto scanResult)
        {
            try
            {
                _logger.LogInformation("🚀 PROCESANDO ESCANEO AZURE: {Archivo}", scanResult?.Archivo);

                // 1️⃣ VALIDAR DATOS MÍNIMOS
                if (!_azureMapper.ValidateMinimumData(scanResult))
                {
                    _logger.LogWarning("⚠️ Escaneo no tiene datos mínimos requeridos");

                    return BadRequest(new
                    {
                        success = false,
                        message = "El escaneo no contiene los datos mínimos requeridos",
                        porcentajeCompletitud = scanResult?.PorcentajeCompletitud ?? 0,
                        camposFaltantes = scanResult?.DatosVelneo?.Metricas?.CamposFaltantes,
                        datosExtraidos = new
                        {
                            numeroPoliza = scanResult?.DatosVelneo?.DatosPoliza?.NumeroPoliza,
                            asegurado = scanResult?.DatosVelneo?.DatosBasicos?.Asegurado,
                            documento = scanResult?.DatosVelneo?.DatosBasicos?.Documento
                        }
                    });
                }

                // 2️⃣ MAPEAR AZURE → POLIZA CREATE REQUEST
                _logger.LogInformation("📋 Mapeando datos escaneados a formato Velneo");
                var createRequest = await _azureMapper.MapAzureResultToCreateRequest(scanResult);

                // 3️⃣ LOG DE VALIDACIÓN
                _logger.LogInformation(@"
            ✅ DATOS MAPEADOS CORRECTAMENTE:
            - Póliza: {NumeroPoliza}
            - Cliente: {Cliente} (ID: {ClienteId})
            - Vehículo: {Vehiculo} {Anio}
            - Premio: {Premio:C}
            - Cuotas: {Cuotas}
            - Compañía: {Compania}",
                    createRequest.Conpol,
                    createRequest.Clinom,
                    createRequest.Clinro,
                    createRequest.Conmaraut,
                    createRequest.Conanioaut,
                    createRequest.Conpremio,
                    createRequest.Concuo,
                    createRequest.Com_alias);

                // 4️⃣ CREAR PÓLIZA EN VELNEO
                _logger.LogInformation("📤 Enviando póliza a Velneo...");
                var resultado = await _velneoMaestrosService.CreatePolizaFromRequestAsync(createRequest);

                // 5️⃣ RESPONDER AL FRONTEND
                _logger.LogInformation("✅ PÓLIZA CREADA EXITOSAMENTE: {NumeroPoliza}", createRequest.Conpol);

                return Ok(new
                {
                    success = true,
                    message = "Póliza creada exitosamente en Velneo desde escaneo",
                    data = new
                    {
                        numeroPoliza = createRequest.Conpol,
                        clienteId = createRequest.Clinro,
                        cliente = createRequest.Clinom,
                        fechaDesde = createRequest.Confchdes,
                        fechaHasta = createRequest.Confchhas,
                        premio = createRequest.Conpremio,
                        total = createRequest.Contot,
                        vehiculo = new
                        {
                            marca = createRequest.Marca,
                            modelo = createRequest.Modelo,
                            anio = createRequest.Conanioaut,
                            matricula = createRequest.Conmataut,
                            chasis = createRequest.Conchasis,
                            motor = createRequest.Conmotor
                        },
                        velneoResponse = resultado
                    },
                    procesadoConIA = true,
                    archivoOrigen = scanResult.Archivo,
                    tiempoProcesamiento = scanResult.TiempoProcesamiento,
                    porcentajeCompletitud = scanResult.PorcentajeCompletitud,
                    timestamp = DateTime.Now
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "❌ Error de validación al procesar escaneo");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    type = "validation_error"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error interno al procesar escaneo");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno al procesar el escaneo",
                    error = ex.Message,
                    type = "internal_error"
                });
            }
        }

        /// <summary>
        /// ✅ NUEVO: Validar datos escaneados antes de enviar
        /// </summary>
        [HttpPost("validar-escaneo")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult> ValidarEscaneo([FromBody] AzureProcessResponseDto scanResult)
        {
            try
            {
                var esValido = _azureMapper.ValidateMinimumData(scanResult);

                if (esValido)
                {
                    // Intentar mapear para detectar posibles problemas
                    var createRequest = await _azureMapper.MapAzureResultToCreateRequest(scanResult);

                    return Ok(new
                    {
                        success = true,
                        valido = true,
                        message = "Datos válidos para procesar",
                        datos = new
                        {
                            numeroPoliza = createRequest.Conpol,
                            cliente = createRequest.Clinom,
                            premio = createRequest.Conpremio,
                            porcentajeCompletitud = scanResult.PorcentajeCompletitud
                        },
                        advertencias = GenerarAdvertenciasEscaneo(scanResult)
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        valido = false,
                        message = "Datos incompletos o inválidos",
                        porcentajeCompletitud = scanResult?.PorcentajeCompletitud ?? 0,
                        camposFaltantes = scanResult?.DatosVelneo?.Metricas?.CamposFaltantes,
                        camposConBajaConfianza = scanResult?.DatosVelneo?.Metricas?.CamposConfianzaBaja,
                        datosExtraidos = new
                        {
                            numeroPoliza = scanResult?.DatosVelneo?.DatosPoliza?.NumeroPoliza,
                            asegurado = scanResult?.DatosVelneo?.DatosBasicos?.Asegurado,
                            documento = scanResult?.DatosVelneo?.DatosBasicos?.Documento,
                            vehiculo = scanResult?.DatosVelneo?.DatosVehiculo?.MarcaModelo
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar escaneo");
                return Ok(new
                {
                    success = false,
                    valido = false,
                    message = "Error al validar datos",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ✅ NUEVO: Validar mapeo completo con datos maestros
        /// </summary>
        [HttpPost("validar-mapeo")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult> ValidarMapeoCompleto([FromBody] AzureProcessResponseDto scanResult)
        {
            try
            {
                _logger.LogInformation("🔍 Validando mapeo completo para escaneo");

                // Usar el servicio de maestros para validar mapeo
                var resultadoMapeo = await _velneoMaestrosService.ValidarMapeoCompletoAsync(scanResult.DatosVelneo);

                return Ok(new
                {
                    success = true,
                    porcentajeExito = resultadoMapeo.PorcentajeExito,
                    camposMapeados = resultadoMapeo.CamposMapeados.Count,
                    camposConAltaConfianza = resultadoMapeo.CamposConAltaConfianza,
                    camposConMediaConfianza = resultadoMapeo.CamposConMediaConfianza,
                    camposConBajaConfianza = resultadoMapeo.CamposConBajaConfianza,
                    camposQueFallaronMapeo = resultadoMapeo.CamposQueFallaronMapeo,
                    detalleMapeo = resultadoMapeo.CamposMapeados,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar mapeo");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al validar mapeo",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ✅ NUEVO: Obtener estado del procesamiento
        /// </summary>
        [HttpGet("estado-procesamiento/{numeroPoliza}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult> GetEstadoProcesamiento(string numeroPoliza)
        {
            try
            {
                // Buscar la póliza en Velneo
                var polizas = await _velneoMaestrosService.GetPolizasAsync();
                var poliza = polizas.FirstOrDefault(p => p.Conpol == numeroPoliza);

                if (poliza != null)
                {
                    return Ok(new
                    {
                        success = true,
                        numeroPoliza = numeroPoliza,
                        estado = "PROCESADO",
                        existeEnVelneo = true,
                        fechaProcesamiento = poliza.Ingresado,
                        detalles = new
                        {
                            id = poliza.Id,
                            cliente = poliza.Clinom,
                            compania = poliza.Com_alias,
                            estado = poliza.Convig,
                            premio = poliza.Conpremio,
                            total = poliza.Contot
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = true,
                        numeroPoliza = numeroPoliza,
                        estado = "NO_ENCONTRADO",
                        existeEnVelneo = false,
                        mensaje = "La póliza no se encuentra en Velneo"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener estado",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region MÉTODOS AUXILIARES PARA AZURE

        private List<string> GenerarAdvertenciasEscaneo(AzureProcessResponseDto scanResult)
        {
            var advertencias = new List<string>();

            if (scanResult.PorcentajeCompletitud < 80)
                advertencias.Add($"Porcentaje de completitud bajo: {scanResult.PorcentajeCompletitud}%");

            if (scanResult.DatosVelneo?.Metricas?.CamposConfianzaBaja?.Any() == true)
                advertencias.Add($"Campos con baja confianza: {string.Join(", ", scanResult.DatosVelneo.Metricas.CamposConfianzaBaja)}");

            if (string.IsNullOrEmpty(scanResult.DatosVelneo?.DatosVehiculo?.Matricula))
                advertencias.Add("Matrícula del vehículo no detectada");

            if (string.IsNullOrEmpty(scanResult.DatosVelneo?.DatosVehiculo?.Chasis))
                advertencias.Add("Chasis del vehículo no detectado");

            if (scanResult.DatosVelneo?.CondicionesPago?.DetalleCuotas?.TieneCuotasDetalladas == false)
                advertencias.Add("Detalle de cuotas no disponible");

            return advertencias;
        }

        #endregion

        #region MÉTODOS AUXILIARES (MANTENIDOS)

        private void LogDatosRecibidos(PolizaCreateRequest request)
        {
            _logger.LogInformation("📊 DATOS RECIBIDOS PARA PÓLIZA {NumeroPoliza}:", request.Conpol);
            _logger.LogInformation("   🏢 Compañía: {Comcod}", request.Comcod);
            _logger.LogInformation("   📋 Sección: {Seccod} / {SeccionId}", request.Seccod, request.SeccionId);
            _logger.LogInformation("   👤 Cliente: {Clinro} - {Asegurado}", request.Clinro, request.Asegurado);
            _logger.LogInformation("   📅 Vigencia: {Desde} a {Hasta}", request.Confchdes, request.Confchhas);
            _logger.LogInformation("   💰 Premio: {Premio} - Total: {Total}", request.Conpremio, request.Contot);
            _logger.LogInformation("   🚗 Vehículo: {Marca} {Anio} - {Matricula}", request.Conmaraut ?? request.Marca, request.Conanioaut ?? request.Anio, request.Conmataut ?? request.Matricula);
            _logger.LogInformation("   📄 Trámite: {Tramite} - Estado: {Estado}", request.Contra ?? request.Tramite, request.Convig ?? request.EstadoPoliza);
            _logger.LogInformation("   💳 Forma Pago: {FormaPago} - Cuotas: {Cuotas}", request.Consta ?? request.FormaPago, request.Concuo ?? request.CantidadCuotas);
            _logger.LogInformation("   🤖 Procesado con IA: {ProcesadoConIA}", request.ProcesadoConIA);
        }

        private async Task ProcesarCamposAdicionales(PolizaCreateRequest request)
        {
            SincronizarCamposDuplicados(request);
            AplicarValoresPorDefecto(request);
            await ValidarConsistenciaDatos(request);
            _logger.LogInformation("✅ Campos adicionales procesados para póliza {NumeroPoliza}", request.Conpol);
        }

        private void SincronizarCamposDuplicados(PolizaCreateRequest request)
        {
            if (request.Seccod > 0 && (!request.SeccionId.HasValue || request.SeccionId <= 0))
                request.SeccionId = request.Seccod;
            else if (request.SeccionId.HasValue && request.SeccionId > 0 && request.Seccod <= 0)
                request.Seccod = request.SeccionId.Value;

            if (string.IsNullOrEmpty(request.Conmaraut) && !string.IsNullOrEmpty(request.Marca))
                request.Conmaraut = !string.IsNullOrEmpty(request.Modelo) ? $"{request.Marca} {request.Modelo}".Trim() : request.Marca;

            if (!request.Conanioaut.HasValue && request.Anio.HasValue)
                request.Conanioaut = request.Anio;

            if (string.IsNullOrEmpty(request.Conmataut) && !string.IsNullOrEmpty(request.Matricula))
                request.Conmataut = request.Matricula;

            if (string.IsNullOrEmpty(request.Conmotor) && !string.IsNullOrEmpty(request.Motor))
                request.Conmotor = request.Motor;

            if (string.IsNullOrEmpty(request.Conchasis) && !string.IsNullOrEmpty(request.Chasis))
                request.Conchasis = request.Chasis;

            if (!request.Contot.HasValue && request.PremioTotal.HasValue)
                request.Contot = request.PremioTotal;

            if (!request.Concuo.HasValue && request.CantidadCuotas.HasValue)
                request.Concuo = request.CantidadCuotas;

            if (string.IsNullOrEmpty(request.Condom) && !string.IsNullOrEmpty(request.Direccion))
                request.Condom = request.Direccion;

            if (string.IsNullOrEmpty(request.Clinom) && !string.IsNullOrEmpty(request.Asegurado))
                request.Clinom = request.Asegurado;
        }

        private void AplicarValoresPorDefecto(PolizaCreateRequest request)
        {
            request.Ramo ??= "AUTOMOVILES";
            request.Congesti ??= "1";
            request.Congeses ??= "1";
            request.Convig ??= "VIG";
            request.Consta ??= "1";

            if (string.IsNullOrEmpty(request.Confchdes))
                request.Confchdes = DateTime.Now.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(request.Confchhas))
                request.Confchhas = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            request.Concuo ??= 1;
            request.Moncod ??= 1;

            if (!request.Conanioaut.HasValue || request.Conanioaut <= 0)
                request.Conanioaut = DateTime.Now.Year;

            if (!request.Contot.HasValue || request.Contot <= 0)
                request.Contot = request.Conpremio;
        }

        private async Task ValidarConsistenciaDatos(PolizaCreateRequest request)
        {
            await Task.CompletedTask;
        }

        private int ResolverSeccionParaRespuesta(PolizaCreateRequest request)
        {
            return request.Seccod > 0 ? request.Seccod : request.SeccionId ?? 0;
        }

        private object GenerarResumenDatos(PolizaCreateRequest request)
        {
            return new
            {
                datosBasicos = new
                {
                    numeroPoliza = request.Conpol,
                    compania = request.Comcod,
                    seccion = ResolverSeccionParaRespuesta(request),
                    cliente = request.Clinro,
                    asegurado = request.Asegurado,
                    premio = request.Conpremio,
                    total = request.Contot,
                    moneda = request.Moncod
                },
                vehiculo = new
                {
                    marca = request.Conmaraut,
                    anio = request.Conanioaut,
                    matricula = request.Conmataut,
                    motor = request.Conmotor,
                    chasis = request.Conchasis
                },
                gestion = new
                {
                    tramite = request.Contra,
                    estadoPoliza = request.Convig,
                    estadoGestion = request.Congeses,
                    formaPago = request.Consta,
                    cuotas = request.Concuo
                },
                vigencia = new
                {
                    desde = request.Confchdes,
                    hasta = request.Confchhas
                }
            };
        }
        #endregion
    }
}