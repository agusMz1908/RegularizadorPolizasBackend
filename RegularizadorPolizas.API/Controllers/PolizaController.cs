using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces.External;
using System.ComponentModel.DataAnnotations;

namespace RegularizadorPolizas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PolizaController : ControllerBase
    {
        // ✅ CAMBIO: Usar interfaz en lugar del tipo concreto
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<PolizaController> _logger;

        public PolizaController(
            IVelneoApiService velneoApiService, // ✅ CAMBIO: Interfaz
            ILogger<PolizaController> logger)
        {
            _velneoApiService = velneoApiService;
            _logger = logger;
        }

        /// <summary>
        /// Crear nueva póliza en Velneo con mapeo completo de campos
        /// </summary>
        /// <param name="request">Datos de la póliza a crear</param>
        /// <returns>Resultado de la creación</returns>
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

                // ✅ ENVIAR A VELNEO CON EL NUEVO MÉTODO COMPLETO
                var resultado = await _velneoApiService.CreatePolizaFromRequestAsync(request);

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
                            procesadoConIA = request.ProcesadoConIA
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetPolizaById(int id)
        {
            try
            {
                _logger.LogInformation("🔍 Buscando póliza: {PolizaId}", id);

                var poliza = await _velneoApiService.GetPolizaAsync(id);

                if (poliza == null)
                {
                    _logger.LogWarning("🚫 Póliza no encontrada: {PolizaId}", id);
                    return NotFound(new { message = $"Póliza {id} no encontrada" });
                }

                return Ok(poliza);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al buscar póliza {PolizaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Validar datos de póliza sin crear (dry-run)
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

        #region MÉTODOS AUXILIARES

        /// <summary>
        /// Log detallado de los datos recibidos para debugging
        /// </summary>
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

        /// <summary>
        /// Procesar campos adicionales antes del envío
        /// </summary>
        private async Task ProcesarCamposAdicionales(PolizaCreateRequest request)
        {
            // ✅ SINCRONIZAR CAMPOS DUPLICADOS
            SincronizarCamposDuplicados(request);

            // ✅ APLICAR DEFAULTS
            AplicarValoresPorDefecto(request);

            // ✅ VALIDAR CONSISTENCIA
            await ValidarConsistenciaDatos(request);

            _logger.LogInformation("✅ Campos adicionales procesados para póliza {NumeroPoliza}", request.Conpol);
        }

        /// <summary>
        /// Sincronizar campos duplicados entre versiones legacy y nuevas
        /// </summary>
        private void SincronizarCamposDuplicados(PolizaCreateRequest request)
        {
            // Sincronizar sección
            if (request.Seccod > 0 && (!request.SeccionId.HasValue || request.SeccionId <= 0))
                request.SeccionId = request.Seccod;
            else if (request.SeccionId.HasValue && request.SeccionId > 0 && request.Seccod <= 0)
                request.Seccod = request.SeccionId.Value;

            // Sincronizar marca/vehículo
            if (string.IsNullOrEmpty(request.Conmaraut) && !string.IsNullOrEmpty(request.Marca))
                request.Conmaraut = !string.IsNullOrEmpty(request.Modelo) ? $"{request.Marca} {request.Modelo}".Trim() : request.Marca;

            // Sincronizar año
            if (!request.Conanioaut.HasValue && request.Anio.HasValue)
                request.Conanioaut = request.Anio;

            // Sincronizar matrícula
            if (string.IsNullOrEmpty(request.Conmataut) && !string.IsNullOrEmpty(request.Matricula))
                request.Conmataut = request.Matricula;

            // Sincronizar motor
            if (string.IsNullOrEmpty(request.Conmotor) && !string.IsNullOrEmpty(request.Motor))
                request.Conmotor = request.Motor;

            // Sincronizar chasis
            if (string.IsNullOrEmpty(request.Conchasis) && !string.IsNullOrEmpty(request.Chasis))
                request.Conchasis = request.Chasis;

            // Sincronizar total
            if (!request.Contot.HasValue && request.PremioTotal.HasValue)
                request.Contot = request.PremioTotal;

            // Sincronizar cuotas
            if (!request.Concuo.HasValue && request.CantidadCuotas.HasValue)
                request.Concuo = request.CantidadCuotas;

            // Sincronizar dirección
            if (string.IsNullOrEmpty(request.Condom) && !string.IsNullOrEmpty(request.Direccion))
                request.Condom = request.Direccion;

            // Sincronizar nombre cliente
            if (string.IsNullOrEmpty(request.Clinom) && !string.IsNullOrEmpty(request.Asegurado))
                request.Clinom = request.Asegurado;
        }

        /// <summary>
        /// Aplicar valores por defecto
        /// </summary>
        private void AplicarValoresPorDefecto(PolizaCreateRequest request)
        {
            // Defaults básicos
            request.Ramo ??= "AUTOMOVILES";
            request.Congesti ??= "1"; // Tipo gestión por defecto
            request.Congeses ??= "1"; // Estado gestión: Pendiente
            request.Convig ??= "VIG"; // Estado póliza: Vigente
            request.Consta ??= "1";   // Forma pago: Contado

            // Defaults de fechas
            if (string.IsNullOrEmpty(request.Confchdes))
                request.Confchdes = DateTime.Now.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(request.Confchhas))
                request.Confchhas = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            // Defaults de cuotas y moneda
            request.Concuo ??= 1;
            request.Moncod ??= 1; // UYU

            // Default de año si está vacío
            if (!request.Conanioaut.HasValue || request.Conanioaut <= 0)
                request.Conanioaut = DateTime.Now.Year;

            // Default de total si está vacío
            if (!request.Contot.HasValue || request.Contot <= 0)
                request.Contot = request.Conpremio;
        }

        /// <summary>
        /// Validar consistencia de datos
        /// </summary>
        private async Task ValidarConsistenciaDatos(PolizaCreateRequest request)
        {
            // TODO: Implementar validaciones de consistencia
            // - Verificar que el cliente existe en Velneo
            // - Verificar que la compañía existe
            // - Validar que las fechas sean coherentes
            // - Validar montos
            await Task.CompletedTask;
        }

        /// <summary>
        /// Resolver sección para respuesta
        /// </summary>
        private int ResolverSeccionParaRespuesta(PolizaCreateRequest request)
        {
            return request.Seccod > 0 ? request.Seccod : request.SeccionId ?? 0;
        }

        /// <summary>
        /// Generar resumen de datos enviados
        /// </summary>
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

        /// <summary>
        /// Generar resumen de validación
        /// </summary>
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

        /// <summary>
        /// Generar advertencias
        /// </summary>
        private List<string> GenerarAdvertencias(PolizaCreateRequest request)
        {
            var advertencias = new List<string>();

            // Advertencias por campos faltantes importantes
            if (request.Seccod <= 0 && (!request.SeccionId.HasValue || request.SeccionId <= 0))
                advertencias.Add("Sección no especificada");

            if (string.IsNullOrWhiteSpace(request.Conmaraut) && string.IsNullOrWhiteSpace(request.Marca))
                advertencias.Add("Marca del vehículo no especificada");

            if (!request.Conanioaut.HasValue && !request.Anio.HasValue)
                advertencias.Add("Año del vehículo no especificado");

            if (string.IsNullOrWhiteSpace(request.Conmataut) && string.IsNullOrWhiteSpace(request.Matricula))
                advertencias.Add("Matrícula del vehículo no especificada");

            // Advertencias por inconsistencias
            if (request.Contot.HasValue && request.Contot < request.Conpremio)
                advertencias.Add("El total es menor que el premio base");

            if (request.Concuo.HasValue && request.Concuo > 1 && request.Consta == "1")
                advertencias.Add("Forma de pago es contado pero se especificaron cuotas");

            return advertencias;
        }

        #endregion
    }
}