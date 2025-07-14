using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IProcessDocumentService _processDocumentService;
        private readonly IPolizaService _polizaService;

        public DocumentsController(
            IProcessDocumentService processDocumentService,
            IPolizaService polizaService)
        {
            _processDocumentService = processDocumentService ?? throw new ArgumentNullException(nameof(processDocumentService));
            _polizaService = polizaService ?? throw new ArgumentNullException(nameof(polizaService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProcessingDocumentDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProcessingDocumentDto>>> GetAllDocuments()
        {
            try
            {
                var documents = await _processDocumentService.GetAllProcessedDocumentsAsync();
                if (documents == null || !documents.Any())
                    return NotFound("No documents found");

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProcessingDocumentDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProcessingDocumentDto>> GetDocumentById(int id)
        {
            try
            {
                var document = await _processDocumentService.GetDocumentProcessingResultAsync(id);
                if (document == null)
                    return NotFound($"Document with ID {id} not found");

                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<ProcessingDocumentDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProcessingDocumentDto>>> GetDocumentsByStatus(string status)
        {
            try
            {
                var documents = await _processDocumentService.GetDocumentsByStatusAsync(status);
                if (documents == null || !documents.Any())
                    return NotFound($"No documents found with status '{status}'");

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("policy/{polizaId}")]
        [ProducesResponseType(typeof(IEnumerable<ProcessingDocumentDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProcessingDocumentDto>>> GetDocumentsByPolicy(int polizaId)
        {
            try
            {
                var documents = await _processDocumentService.GetDocumentsByPolizaAsync(polizaId);
                if (documents == null || !documents.Any())
                    return NotFound($"No documents found for policy with ID {polizaId}");

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(DocumentResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentResultDto>> UploadAndProcessDocument(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var allowedExtensions = new[] { ".pdf" };
                var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Only PDF files are allowed");

                var result = await _processDocumentService.ProcessDocumentAsync(file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}/extract")]
        [ProducesResponseType(typeof(PolizaDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PolizaDto>> ExtractPolicyFromDocument(int id)
        {
            try
            {
                var poliza = await _processDocumentService.ExtractPolizaFromDocumentAsync(id);
                if (poliza == null)
                    return NotFound($"Could not extract policy data from document with ID {id}");

                return Ok(poliza);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/create-policy")]
        [ProducesResponseType(typeof(PolizaDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PolizaDto>> CreatePolicyFromDocument(int id)
        {
            try
            {
                // Extract policy data from document
                var polizaDto = await _processDocumentService.ExtractPolizaFromDocumentAsync(id);
                if (polizaDto == null)
                    return NotFound($"Could not extract policy data from document with ID {id}");

                // Create policy
                var createdPoliza = await _polizaService.CreatePolizaAsync(polizaDto);

                // Link document to policy
                await _processDocumentService.LinkDocumentToPolizaAsync(id, createdPoliza.Id);

                return CreatedAtAction("GetPolizaById", "Polizas", new { id = createdPoliza.Id }, createdPoliza);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/link/{polizaId}")]
        [ProducesResponseType(typeof(ProcessingDocumentDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProcessingDocumentDto>> LinkDocumentToPolicy(int id, int polizaId)
        {
            try
            {
                var document = await _processDocumentService.LinkDocumentToPolizaAsync(id, polizaId);
                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}