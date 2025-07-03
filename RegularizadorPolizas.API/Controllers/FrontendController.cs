using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Frontend;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.Services.External;
using System.Security.Claims;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/frontend")]
    [Authorize]
    public class FrontendController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IPolizaService _polizaService;
        private readonly IAzureDocumentIntelligenceService _documentService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IVelneoApiService _velneoApiService;
        private readonly IHybridApiService _hybridApiService;
        private readonly ILogger<FrontendController> _logger;

        public FrontendController(
            IClientService clientService,
            IPolizaService polizaService,
            IAzureDocumentIntelligenceService documentService,
            IFileStorageService fileStorageService,
            IVelneoApiService velneoApiService,
            IHybridApiService hybridApiService,
            ILogger<FrontendController> logger)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _polizaService = polizaService ?? throw new ArgumentNullException(nameof(polizaService));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("clients")]
        [ProducesResponseType(typeof(IEnumerable<ClientSummaryDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClientSummaryDto>>> GetClients(
            [FromQuery] string search = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 50;

                _logger.LogInformation("Getting clients list. Search: {Search}, Page: {Page}, PageSize: {PageSize}",
                    search, page, pageSize);

                var clients = string.IsNullOrEmpty(search)
                    ? await _clientService.GetAllClientsAsync()
                    : await _clientService.SearchClientsAsync(search);

                var clientsSummary = clients.Select(c => new ClientSummaryDto
                {
                    Id = c.Id,
                    Nombre = c.Clinom,
                    Documento = !string.IsNullOrEmpty(c.Cliruc) ? c.Cliruc : c.Cliced,
                    Rut = c.Cliruc,
                    Telefono = c.Telefono,
                    Celular = c.Clitelcel,
                    Domicilio = c.Clidir,
                    Email = c.Cliemail,
                    TotalPolizas = 0,
                    TienePolizasVigentes = false, 
                    EstadoCliente = c.Activo ? "Activo" : "Inactivo",
                    TipoCliente = !string.IsNullOrEmpty(c.Cliruc) ? "Empresa" : "Persona",
                    RazonSocial = c.Clirsoc
                })
                                .Skip((page - 1) * pageSize)
                .Take(pageSize);

                return Ok(clientsSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients list");
                return StatusCode(500, $"Error retrieving clients: {ex.Message}");
            }
        }

        [HttpGet("clients/{clientId}/polizas")]
        [ProducesResponseType(typeof(IEnumerable<PolizaSummaryDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PolizaSummaryDto>>> GetClientPolizas(int clientId)
        {
            try
            {
                _logger.LogInformation("Getting polizas for client {ClientId}", clientId);

                var cliente = await _clientService.GetClientByIdAsync(clientId);
                if (cliente == null)
                {
                    return NotFound($"Cliente con ID {clientId} no encontrado");
                }

                var polizas = await _polizaService.GetPolizasByClienteAsync(clientId);
                var polizasSummary = polizas.Select(p => new PolizaSummaryDto
                {
                    Id = p.Id,
                    Numero = p.Conpol,
                    Desde = p.Confchdes,
                    Hasta = p.Confchhas,
                    //Compania = p.Comcod,
                    //Ramo = DetermineRamoName(p.Seccod),
                    Estado = DeterminePolizaEstado(p),
                    Prima = p.Conpremio,
                    Moneda = DetermineMoneda(p.Moncod),
                    RequiereAtencion = IsPolizaRequiereAtencion(p),
                    //TipoOperacion = DetermineTipoOperacion(p),
                    Vigencia = p.Convig,
                    Endoso = p.Conend,
                    Total = p.Contot
                });

                return Ok(polizasSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving polizas for client {ClientId}", clientId);
                return StatusCode(500, $"Error retrieving client policies: {ex.Message}");
            }
        }

        [HttpPost("process-poliza")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(DocumentProcessResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentProcessResultDto>> ProcessPoliza(
            int compañiaId,        
            IFormFile archivo)        
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return BadRequest("No se proporcionó ningún archivo");

                if (!archivo.ContentType.Contains("pdf") && !archivo.FileName.ToLower().EndsWith(".pdf"))
                    return BadRequest("Solo se admiten archivos PDF");

                if (archivo.Length > 10 * 1024 * 1024) // 10MB
                    return BadRequest("El archivo es demasiado grande. Máximo 10MB");

                _logger.LogInformation("Processing poliza document {FileName} for company {CompanyId}",
                    archivo.FileName, compañiaId);

                var company = await _hybridApiService.GetCompanyByIdAsync(compañiaId);
                if (company == null)
                    return BadRequest($"Compañía con ID {compañiaId} no encontrada");
                var modelId = DetermineModelByCompany(company.Cod_srvcompanias);

                var azureResult = await _documentService.ProcessDocumentAsync(archivo, modelId);

                if (azureResult.EstadoProcesamiento != "PROCESADO")
                {
                    return BadRequest($"Error procesando documento: {azureResult.MensajeError}");
                }

                var pdfUrl = await _fileStorageService.SavePdfAsync(archivo, 0); 

                if (azureResult.PolizaProcesada != null)
                {
                    azureResult.PolizaProcesada.Comcod = compañiaId;

                    var result = new DocumentProcessResultDto
                    {
                        NombreArchivo = azureResult.NombreArchivo,
                        EstadoProcesamiento = azureResult.EstadoProcesamiento,
                        ConfianzaExtraccion = azureResult.ConfianzaExtraccion,
                        TiempoProcesamiento = azureResult.TiempoProcesamiento,
                        RequiereRevision = azureResult.RequiereRevision,
                        FechaProcesamiento = azureResult.FechaProcesamiento,

                        PdfViewerUrl = pdfUrl,
                        PolizaDatos = azureResult.PolizaProcesada,

                        RequiereVerificacion = azureResult.ConfianzaExtraccion < 0.85m,
                        CamposConBajaConfianza = GetLowConfidenceFields(azureResult),
                        ConfianzaPorCampo = GetConfidencePerField(azureResult),
                        EstadoFormulario = "pendiente",

                        CompaniaSeleccionada = company.Comnom,
                        TipoOperacion = "Nueva", 
                        TipoDocumentoDetectado = "Póliza de Seguro"
                    };

                    _logger.LogInformation("Poliza processed successfully. Company: {Company}, Confidence: {Confidence}%",
                        company.Comnom, azureResult.ConfianzaExtraccion * 100);

                    return Ok(result);
                }

                return BadRequest("No se pudieron extraer datos válidos del documento");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing poliza {FileName} for company {CompanyId}",
                    archivo?.FileName, compañiaId);
                return StatusCode(500, $"Error interno procesando póliza: {ex.Message}");
            }
        }

        private string DetermineModelByCompany(string companyCod)
        {
            return companyCod switch
            {
                "BSE" => "poliza-vehiculo-bse",
                "SURA" => "poliza-vehiculo-sura",    
                "MAPFRE" => "poliza-vehiculo-mapfre", 
                "PORTO" => "poliza-vehiculo-porto",  
                _ => "poliza-vehiculo-bse" // Fallback al modelo BSE
            };
        }

        [HttpPost("send-to-velneo")]
        [ProducesResponseType(typeof(VelneoSendResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VelneoSendResultDto>> SendToVelneo([FromBody] PolizaDto polizaData)
        {
            try
            {
                if (polizaData == null)
                    return BadRequest("No se proporcionaron datos de la póliza");

                var userId = GetCurrentUserId();
                _logger.LogInformation("User {UserId} sending poliza {PolizaNumber} to Velneo", userId, polizaData.Conpol);

                var validationResult = ValidatePolizaForVelneo(polizaData);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new VelneoSendResultDto
                    {
                        Success = false,
                        ErrorMessage = "Validación fallida",
                        ValidationErrors = validationResult.Errors
                    });
                }

                var transactionId = Guid.NewGuid().ToString();

                try
                {
                    var velneoResult = await _velneoApiService.CreatePolizaAsync(polizaData);

                    if (velneoResult != null)
                    {
                        var localPoliza = await _polizaService.CreatePolizaAsync(polizaData);

                        return Ok(new VelneoSendResultDto
                        {
                            Success = true,
                            VelneoPolizaId = velneoResult.Id.ToString(),
                            LocalPolizaId = localPoliza.Id,
                            Message = "Póliza enviada exitosamente a Velneo",
                            TransactionId = transactionId,
                            RequiereVerificacionManual = polizaData.Conpremio == null || polizaData.Conpremio <= 0
                        });
                    }
                    else
                    {
                        return StatusCode(500, new VelneoSendResultDto
                        {
                            Success = false,
                            ErrorMessage = "Velneo devolvió una respuesta vacía",
                            TransactionId = transactionId
                        });
                    }
                }
                catch (Exception velneoEx)
                {
                    _logger.LogError(velneoEx, "Error comunicating with Velneo API");
                    return StatusCode(500, new VelneoSendResultDto
                    {
                        Success = false,
                        ErrorMessage = $"Error comunicándose con Velneo: {velneoEx.Message}",
                        TransactionId = transactionId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending poliza to Velneo");
                return StatusCode(500, new VelneoSendResultDto
                {
                    Success = false,
                    ErrorMessage = $"Error interno: {ex.Message}"
                });
            }
        }

        #region Métodos Auxiliares Privados

        private string DeterminePolizaEstado(PolizaDto poliza)
        {
            if (!poliza.Confchhas.HasValue)
                return "Nuevo";

            var today = DateTime.Now.Date;

            if (poliza.Confchhas.Value.Date < today)
                return "Vencido";

            if (poliza.Confchdes.HasValue && poliza.Confchdes.Value.Date <= today && poliza.Confchhas.Value.Date >= today)
                return "Vigente";

            return "Nuevo";
        }

        private string DetermineMoneda(int? monedaCode)
        {
            return monedaCode switch
            {
                1 => "UYU",
                2 => "USD",
                _ => "UYU"
            };
        }

        private bool IsPolizaRequiereAtencion(PolizaDto poliza)
        {
            if (!poliza.Confchhas.HasValue) return true;

            var diasParaVencimiento = (poliza.Confchhas.Value - DateTime.Now).Days;
            return diasParaVencimiento <= 30 && diasParaVencimiento >= 0;
        }

        private List<string> GetLowConfidenceFields(DocumentResultDto azureResult)
        {
            var lowConfidenceFields = new List<string>();

            if (azureResult.ConfianzaExtraccion < 0.7m)
            {
                lowConfidenceFields.AddRange(new[] { "Número de Póliza", "Vigencia", "Prima" });
            }
            else if (azureResult.ConfianzaExtraccion < 0.85m)
            {
                lowConfidenceFields.AddRange(new[] { "Prima", "Suma Asegurada" });
            }

            return lowConfidenceFields;
        }

        private Dictionary<string, decimal> GetConfidencePerField(DocumentResultDto azureResult)
        {
            return new Dictionary<string, decimal>
            {
                ["Número de Póliza"] = azureResult.ConfianzaExtraccion,
                ["Cliente"] = azureResult.ConfianzaExtraccion + 0.05m,
                ["Vigencia Desde"] = azureResult.ConfianzaExtraccion - 0.02m,
                ["Vigencia Hasta"] = azureResult.ConfianzaExtraccion - 0.02m,
                ["Prima"] = azureResult.ConfianzaExtraccion - 0.1m,
                ["Compañía"] = azureResult.ConfianzaExtraccion + 0.03m
            };
        }

        private bool DetectIfRenovacion(PolizaDto poliza)
        {
            return poliza.Conpol?.Contains("-R") == true ||
                   poliza.Conpol?.ToUpper().Contains("RENOV") == true;
        }

        private ValidationResult ValidatePolizaForVelneo(PolizaDto poliza)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(poliza.Conpol))
                errors.Add("Número de póliza es requerido");

            if (!poliza.Clinro.HasValue || poliza.Clinro <= 0)
                errors.Add("Cliente es requerido");

            if (!poliza.Confchdes.HasValue)
                errors.Add("Fecha de inicio de vigencia es requerida");

            if (!poliza.Confchhas.HasValue)
                errors.Add("Fecha de fin de vigencia es requerida");

            if (poliza.Confchdes.HasValue && poliza.Confchhas.HasValue &&
                poliza.Confchdes.Value >= poliza.Confchhas.Value)
                errors.Add("La fecha de inicio debe ser menor a la fecha de fin");

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        #endregion
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}