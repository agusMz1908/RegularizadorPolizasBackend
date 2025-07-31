using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External.Velneo;

namespace RegularizadorPolizas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PolizaController : ControllerBase
    {
        private readonly IVelneoMaestrosService _velneoMaestrosService;
        private readonly ILogger<PolizaController> _logger;

        public PolizaController(
            IVelneoMaestrosService velneoMaestrosService, 
            ILogger<PolizaController> logger)
        {
            _velneoMaestrosService = velneoMaestrosService;
            _logger = logger;
        }

        #region CRUD ENDPOINTS - CORREGIDOS ✅

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

        /// <summary>
        /// ✅ EXISTENTE: Validar póliza sin crear (dry-run) - Mantenido
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult> ValidatePoliza([FromBody] PolizaCreateRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Datos requeridos" });
                }

                _logger.LogInformation("🔍 VALIDANDO PÓLIZA: {NumeroPoliza}", request.Conpol);

                var resultadoValidacion = new
                {
                    success = ModelState.IsValid,
                    numeroPoliza = request.Conpol,
                    camposValidados = GenerarResumenValidacion(request),
                    errores = ModelState.IsValid ? new List<string>() :
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    warnings = GenerarAdvertencias(request),
                    timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("✅ Validación completada para póliza {NumeroPoliza}: {Resultado}",
                    request.Conpol, resultadoValidacion.success ? "EXITOSA" : "FALLIDA");

                return Ok(resultadoValidacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error validando póliza {NumeroPoliza}", request?.Conpol);
                return StatusCode(500, new { message = "Error en validación", error = ex.Message });
            }
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

        private object GenerarResumenValidacion(PolizaCreateRequest request)
        {
            var camposRequeridos = new Dictionary<string, bool>
            {
                ["comcod"] = request.Comcod > 0,
                ["clinro"] = request.Clinro > 0,
                ["conpol"] = !string.IsNullOrWhiteSpace(request.Conpol),
                ["confchdes"] = !string.IsNullOrWhiteSpace(request.Confchdes),
                ["confchhas"] = !string.IsNullOrWhiteSpace(request.Confchhas),
                ["conpremio"] = request.Conpremio > 0,
                ["asegurado"] = !string.IsNullOrWhiteSpace(request.Asegurado)
            };

            var camposOpcionales = new Dictionary<string, bool>
            {
                ["seccod"] = request.Seccod > 0 || request.SeccionId > 0,
                ["vehiculo"] = !string.IsNullOrWhiteSpace(request.Conmaraut) || !string.IsNullOrWhiteSpace(request.Marca),
                ["matricula"] = !string.IsNullOrWhiteSpace(request.Conmataut) || !string.IsNullOrWhiteSpace(request.Matricula),
                ["anio"] = request.Conanioaut > 0 || request.Anio > 0,
                ["direccion"] = !string.IsNullOrWhiteSpace(request.Condom) || !string.IsNullOrWhiteSpace(request.Direccion)
            };

            return new
            {
                requeridos = camposRequeridos,
                opcionales = camposOpcionales,
                completitud = new
                {
                    requeridosCompletos = camposRequeridos.Values.Count(v => v),
                    totalRequeridos = camposRequeridos.Count,
                    opcionalesCompletos = camposOpcionales.Values.Count(v => v),
                    totalOpcionales = camposOpcionales.Count
                }
            };
        }

        private List<string> GenerarAdvertencias(PolizaCreateRequest request)
        {
            var advertencias = new List<string>();

            if (request.Seccod <= 0 && (!request.SeccionId.HasValue || request.SeccionId <= 0))
                advertencias.Add("Sección no especificada");

            if (string.IsNullOrWhiteSpace(request.Conmaraut) && string.IsNullOrWhiteSpace(request.Marca))
                advertencias.Add("Marca del vehículo no especificada");

            if (!request.Conanioaut.HasValue && !request.Anio.HasValue)
                advertencias.Add("Año del vehículo no especificado");

            if (string.IsNullOrWhiteSpace(request.Conmataut) && string.IsNullOrWhiteSpace(request.Matricula))
                advertencias.Add("Matrícula del vehículo no especificada");

            if (request.Contot.HasValue && request.Contot < request.Conpremio)
                advertencias.Add("El total es menor que el premio base");

            if (request.Concuo.HasValue && request.Concuo > 1 && request.Consta == "1")
                advertencias.Add("Forma de pago es contado pero se especificaron cuotas");

            return advertencias;
        }

        #endregion
    }
}