using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Infrastructure.External.AzureDocumentIntelligence;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IVelneoApiService _velneoApiService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(
            IVelneoApiService velneoApiService,
            ILogger<ClientsController> logger)
        {
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetClientes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("🔄 REAL PAGINATION: Getting clients - Page: {Page}, PageSize: {PageSize}, Search: {Search}",
                    page, pageSize, search);

                // ✅ USAR PAGINACIÓN REAL DE VELNEO
                var velneoResponse = await _velneoApiService.GetClientesPaginatedAsync(page, pageSize, search);

                var result = new
                {
                    items = velneoResponse.Items,
                    totalCount = velneoResponse.TotalCount,
                    currentPage = velneoResponse.PageNumber,
                    pageNumber = velneoResponse.PageNumber,
                    pageSize = velneoResponse.PageSize,
                    totalPages = velneoResponse.TotalPages,
                    hasNextPage = velneoResponse.HasNextPage,
                    hasPreviousPage = velneoResponse.HasPreviousPage,

                    // ✅ Metadatos útiles
                    startItem = velneoResponse.Items.Any() ? ((page - 1) * pageSize + 1) : 0,
                    endItem = Math.Min(page * pageSize, velneoResponse.TotalCount),

                    // ✅ Info de performance
                    requestDuration = velneoResponse.RequestDuration.TotalMilliseconds,
                    dataSource = "velneo_real_pagination",
                    velneoHasMoreData = velneoResponse.VelneoHasMoreData
                };

                _logger.LogInformation("✅ REAL PAGINATION SUCCESS: Page {Page}/{TotalPages} - {Count} clients in {Duration}ms",
                    page, velneoResponse.TotalPages, velneoResponse.Items.Count(), velneoResponse.RequestDuration.TotalMilliseconds);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error with REAL PAGINATION - Page: {Page}, PageSize: {PageSize}", page, pageSize);
                return StatusCode(500, new
                {
                    message = "Error obteniendo clientes con paginación real",
                    error = ex.Message,
                    pagination = "real"
                });
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetAllClientes([FromQuery] string? search = null)
        {
            try
            {
                _logger.LogInformation("🔄 ALL CLIENTS: Getting all clients (fallback to old method) - Search: {Search}", search);

                // ✅ Para "all", usamos el método anterior que ya funciona
                // Porque obtener TODO con paginación real sería muchas llamadas a Velneo
                var allClients = (await _velneoApiService.GetClientesAsync()).ToList();

                // Aplicar búsqueda si existe
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var originalCount = allClients.Count;
                    allClients = allClients.Where(c =>
                        (c.Clinom?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliemail?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliced?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                        (c.Cliruc?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                    ).ToList();

                    _logger.LogInformation("Search filter applied: {FilteredCount} of {OriginalCount} clients",
                        allClients.Count, originalCount);
                }

                var result = new
                {
                    items = allClients,
                    totalCount = allClients.Count,
                    retrievedAt = DateTime.UtcNow,
                    dataSource = "velneo_full_load", 
                    filtered = !string.IsNullOrWhiteSpace(search),
                    note = "This endpoint loads all data from Velneo for backwards compatibility"
                };

                _logger.LogInformation("✅ ALL CLIENTS SUCCESS: {Count} total clients returned", allClients.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting all clients");
                return StatusCode(500, new
                {
                    message = "Error obteniendo todos los clientes",
                    error = ex.Message,
                    dataSource = "velneo_full_load"
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            try
            {
                _logger.LogInformation("Getting client {ClientId} via Velneo API", id);

                var client = await _velneoApiService.GetClienteAsync(id);

                if (client == null)
                {
                    _logger.LogWarning("Client {ClientId} not found in Velneo API", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                _logger.LogInformation("Successfully retrieved client {ClientId} from Velneo API", id);
                return Ok(client);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found in Velneo API", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("GetCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "GetCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {ClientId} from Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> GetClientsCount()
        {
            try
            {
                var clients = await _velneoApiService.GetClientesAsync();
                var count = clients.Count();

                return Ok(new { total = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting clients via Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ClientDto>>> SearchClients([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Término de búsqueda requerido" });
                }

                _logger.LogInformation("Searching clients with term '{SearchTerm}' via Velneo API", searchTerm);

                var clients = await _velneoApiService.SearchClientesAsync(searchTerm);

                _logger.LogInformation("Found {Count} clients matching '{SearchTerm}' in Velneo API", clients.Count(), searchTerm);
                return Ok(clients);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("SearchClientes not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "SearchClientes no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching clients with term '{SearchTerm}' via Velneo API", searchTerm);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClientDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                {
                    return BadRequest(new { message = "Datos del cliente requeridos" });
                }

                _logger.LogInformation("Creating client via Velneo API");

                var createdClient = await _velneoApiService.CreateClienteAsync(clientDto);

                _logger.LogInformation("Successfully created client {ClientId} via Velneo API", createdClient.Id);
                return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("CreateCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "CreateCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client via Velneo API");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> UpdateClient(int id, [FromBody] ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                {
                    return BadRequest(new { message = "Datos del cliente requeridos" });
                }

                if (id != clientDto.Id)
                {
                    return BadRequest(new { message = "ID en la URL no coincide con el ID del cliente" });
                }

                _logger.LogInformation("Updating client {ClientId} via Velneo API", id);

                await _velneoApiService.UpdateClienteAsync(clientDto);

                _logger.LogInformation("Successfully updated client {ClientId} via Velneo API", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found for update in Velneo API", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("UpdateCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "UpdateCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client {ClientId} via Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> DeleteClient(int id)
        {
            try
            {
                _logger.LogInformation("Deleting client {ClientId} via Velneo API", id);

                await _velneoApiService.DeleteClienteAsync(id);

                _logger.LogInformation("Successfully deleted client {ClientId} via Velneo API", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Client {ClientId} not found for deletion in Velneo API", id);
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }
            catch (NotImplementedException ex)
            {
                _logger.LogWarning("DeleteCliente not implemented in Velneo API: {Message}", ex.Message);
                return StatusCode(501, new { message = "DeleteCliente no está implementado en Velneo API aún", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client {ClientId} via Velneo API", id);
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        #region Métodos para Procesamiento de Documentos

        /// <summary>
        /// BÚSQUEDA INTELIGENTE - Para matching automático desde documentos
        /// </summary>
        [HttpPost("intelligent-search")]
        [ProducesResponseType(typeof(ClienteMatchResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<ClienteMatchResult>> IntelligentSearch([FromBody] DatosClienteExtraidos datosExtraidos)
        {
            try
            {
                if (datosExtraidos == null)
                {
                    return BadRequest(new { message = "Datos del cliente requeridos" });
                }

                _logger.LogInformation("🔍 INTELLIGENT SEARCH: Nombre='{Nombre}', Doc='{Documento}'",
                    datosExtraidos.Nombre, datosExtraidos.Documento);

                var clienteMatchingService = HttpContext.RequestServices.GetRequiredService<IClienteMatchingService>();
                var resultado = await clienteMatchingService.BuscarClienteAsync(datosExtraidos);

                _logger.LogInformation("✅ INTELLIGENT SEARCH COMPLETED: {TipoResultado} - {Count} matches",
                    resultado.TipoResultado, resultado.Matches.Count);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda inteligente de cliente");
                return StatusCode(500, new { message = "Error en búsqueda inteligente", error = ex.Message });
            }
        }

        /// <summary>
        /// BÚSQUEDA AVANZADA - Múltiples criterios de búsqueda
        /// </summary>
        [HttpPost("advanced-search")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> AdvancedSearch([FromBody] AdvancedSearchRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Criterios de búsqueda requeridos" });
                }

                _logger.LogInformation("🔍 ADVANCED SEARCH: {Criterios}",
                    string.Join(", ", request.GetNonEmptyCriteria()));

                var clienteMatchingService = HttpContext.RequestServices.GetRequiredService<IClienteMatchingService>();

                var datosParaBusqueda = new DatosClienteExtraidos
                {
                    Nombre = request.Nombre ?? "",
                    Documento = request.Documento ?? "",
                    Email = request.Email ?? "",
                    Telefono = request.Telefono ?? "",
                    Direccion = request.Direccion ?? "",
                    Localidad = request.Localidad ?? ""
                };

                var matches = await clienteMatchingService.BuscarClientesAvanzadoAsync(datosParaBusqueda, request.Limite);

                return Ok(new
                {
                    criteriosBusqueda = request.GetNonEmptyCriteria(),
                    clientesEncontrados = matches.Count,
                    matches = matches.Select(m => new
                    {
                        cliente = new
                        {
                            id = m.Cliente.Clinro,
                            nombre = m.Cliente.Clinom,
                            documento = m.Cliente.Cliruc,
                            telefono = m.Cliente.Clitelcel,
                            email = m.Cliente.Cliemail,
                            direccion = m.Cliente.Clidir,
                            localidad = m.Cliente.Clilocnom,
                            activo = m.Cliente.Activo
                        },
                        score = m.Score,
                        criterio = m.Criterio,
                        coincidencias = m.Coincidencias
                    }).ToList(),
                    sugerencias = matches.Count == 0 ? new[]
                    {
                "Intente buscar solo por apellido",
                "Verifique que el documento esté correcto",
                "Use menos criterios para ampliar la búsqueda",
                "Considere crear un cliente nuevo"
            } : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda avanzada");
                return StatusCode(500, new { message = "Error en búsqueda avanzada", error = ex.Message });
            }
        }

        /// <summary>
        /// SUGERIR CLIENTE NUEVO - Preparar datos para creación desde extracción de documento
        /// </summary>
        [HttpPost("suggest-new")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public ActionResult SuggestNewClient([FromBody] SmartExtractedData datosExtraidos)
        {
            try
            {
                if (datosExtraidos == null)
                {
                    return BadRequest(new { message = "Datos extraídos requeridos" });
                }

                _logger.LogInformation("💡 SUGGESTING NEW CLIENT: {Asegurado} - {Documento}",
                    datosExtraidos.Asegurado, datosExtraidos.Documento);

                var clienteSugerido = new
                {
                    // Datos básicos extraídos del PDF
                    clinom = datosExtraidos.Asegurado,
                    cliruc = datosExtraidos.Documento,
                    cliemail = datosExtraidos.Email,
                    clidir = datosExtraidos.Direccion,
                    clilocnom = datosExtraidos.Localidad,
                    clidptnom = datosExtraidos.Departamento,

                    // Valores por defecto que necesitan completarse
                    activo = true,
                    cliced = "", // CI a completar manualmente
                    clitelcel = "", // Teléfono a completar

                    // Metadatos
                    observaciones = $"Cliente sugerido desde póliza {datosExtraidos.NumeroPoliza} - {DateTime.Now:dd/MM/yyyy HH:mm}",
                    origenSugerencia = "extraccion_pdf"
                };

                return Ok(new
                {
                    estado = "CLIENTE_SUGERIDO",
                    mensaje = $"Datos preparados para crear cliente nuevo: {datosExtraidos.Asegurado}",
                    clienteSugerido = clienteSugerido,
                    validacion = new
                    {
                        camposObligatorios = new[] { "clinom", "cliruc", "cliced" },
                        camposOpcionales = new[] { "cliemail", "clidir", "clitelcel", "clilocnom" },
                        camposCompletos = new[]
                        {
                    !string.IsNullOrEmpty(datosExtraidos.Asegurado) ? "clinom" : null,
                    !string.IsNullOrEmpty(datosExtraidos.Documento) ? "cliruc" : null,
                    !string.IsNullOrEmpty(datosExtraidos.Email) ? "cliemail" : null,
                    !string.IsNullOrEmpty(datosExtraidos.Direccion) ? "clidir" : null
                }.Where(c => c != null).ToArray()
                    },
                    recomendacion = "Revisar y completar campos obligatorios antes de crear",
                    siguientePaso = "completar_datos_y_crear"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sugiriendo cliente nuevo");
                return StatusCode(500, new { message = "Error sugiriendo cliente nuevo", error = ex.Message });
            }
        }

        /// <summary>
        /// BUSCAR POR DOCUMENTO - Método directo para búsqueda por documento específico
        /// </summary>
        [HttpGet("by-document/{documento}")]
        [ProducesResponseType(typeof(ClienteMatchResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<ClienteMatchResult>> SearchByDocument(string documento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documento))
                {
                    return BadRequest(new { message = "Documento requerido" });
                }

                _logger.LogInformation("🔍 SEARCH BY DOCUMENT: {Documento}", documento);

                var clienteMatchingService = HttpContext.RequestServices.GetRequiredService<IClienteMatchingService>();
                var resultado = await clienteMatchingService.BuscarClientePorDocumentoAsync(documento);

                _logger.LogInformation("✅ SEARCH BY DOCUMENT COMPLETED: {TipoResultado} - {Count} matches",
                    resultado.TipoResultado, resultado.Matches.Count);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando cliente por documento");
                return StatusCode(500, new { message = "Error buscando por documento", error = ex.Message });
            }
        }

        #endregion
    }
}