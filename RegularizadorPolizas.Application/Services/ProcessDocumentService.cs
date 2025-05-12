using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class ProcessDocumentService : IProcessDocumentService
    {
        private readonly IProcessDocumentRepository _processDocumentRepository;
        private readonly IPolizaRepository _polizaRepository;
        private readonly IAzureDocumentIntelligenceService _documentIntelligenceService;
        private readonly IMapper _mapper;

        public ProcessDocumentService(
            IProcessDocumentRepository processDocumentRepository,
            IPolizaRepository polizaRepository,
            IAzureDocumentIntelligenceService documentIntelligenceService,
            IMapper mapper)
        {
            _processDocumentRepository = processDocumentRepository;
            _polizaRepository = polizaRepository;
            _documentIntelligenceService = documentIntelligenceService;
            _mapper = mapper;
        }

        public async Task<PolizaDto> ExtractPolizaFromDocumentAsync(int documentId)
        {
            try
            {
                var document = await _processDocumentRepository.GetDocumentWithDetailsAsync(documentId);
                if (document == null)
                {
                    throw new ApplicationException($"Document with ID {documentId} not found");
                }

                if (document.EstadoProcesamiento != "PROCESADO")
                {
                    throw new ApplicationException($"Document with ID {documentId} has not been successfully processed");
                }

                var documentResult = new DocumentResultDto
                {
                    DocumentoId = document.Id,
                    NombreArchivo = document.NombreArchivo,
                    EstadoProcesamiento = document.EstadoProcesamiento,
                    CamposExtraidos = JsonConvert.DeserializeObject<Dictionary<string, string>>(document.ResultadoJson)
                };

                var polizaDto = _documentIntelligenceService.MapDocumentToPoliza(documentResult);

                return polizaDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error extracting policy from document: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ProcessDocumentDto>> GetAllProcessedDocumentsAsync()
        {
            try
            {
                var documents = await _processDocumentRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ProcessDocumentDto>>(documents);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving processed documents: {ex.Message}", ex);
            }
        }

        public async Task<DocumentResultDto> GetDocumentProcessingResultAsync(int documentId)
        {
            try
            {
                var document = await _processDocumentRepository.GetDocumentWithDetailsAsync(documentId);
                if (document == null)
                {
                    return null;
                }

                var resultado = new DocumentResultDto
                {
                    DocumentoId = document.Id,
                    NombreArchivo = document.NombreArchivo,
                    EstadoProcesamiento = document.EstadoProcesamiento,
                    CamposExtraidos = !string.IsNullOrEmpty(document.ResultadoJson)
                        ? JsonConvert.DeserializeObject<Dictionary<string, string>>(document.ResultadoJson)
                        : new Dictionary<string, string>(),
                    RequiereRevision = document.EstadoProcesamiento == "REQUIERE_REVISION"
                };

                return resultado;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving document processing result: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ProcessDocumentDto>> GetDocumentsByPolizaAsync(int polizaId)
        {
            try
            {
                var documents = await _processDocumentRepository.GetDocumentsByPolizaAsync(polizaId);
                return _mapper.Map<IEnumerable<ProcessDocumentDto>>(documents);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving documents for policy {polizaId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ProcessDocumentDto>> GetDocumentsByStatusAsync(string status)
        {
            try
            {
                var documents = await _processDocumentRepository.GetDocumentsByStatusAsync(status);
                return _mapper.Map<IEnumerable<ProcessDocumentDto>>(documents);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving documents with status {status}: {ex.Message}", ex);
            }
        }

        public async Task<ProcessDocumentDto> LinkDocumentToPolizaAsync(int documentId, int polizaId)
        {
            try
            {
                var document = await _processDocumentRepository.GetByIdAsync(documentId);
                if (document == null)
                {
                    throw new ApplicationException($"Document with ID {documentId} not found");
                }

                var poliza = await _polizaRepository.GetByIdAsync(polizaId);
                if (poliza == null)
                {
                    throw new ApplicationException($"Policy with ID {polizaId} not found");
                }

                document.PolizaId = polizaId;
                document.FechaModificacion = DateTime.Now;

                await _processDocumentRepository.UpdateAsync(document);

                return _mapper.Map<ProcessDocumentDto>(document);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error linking document to policy: {ex.Message}", ex);
            }
        }

        public async Task<DocumentResultDto> ProcessDocumentAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or not provided");
                }

                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension != ".pdf")
                {
                    throw new ArgumentException("Only PDF files are supported");
                }

                // Process document with Azure Document Intelligence
                var processingResult = await _documentIntelligenceService.ProcessDocumentAsync(file);

                // Create a unique filename
                var fileName = $"{Guid.NewGuid()}-{file.FileName}";
                var filePath = Path.Combine("Documents", fileName);

                // Create directory if it doesn't exist
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create document entity
                var document = new ProcessDocument
                {
                    NombreArchivo = file.FileName,
                    RutaArchivo = filePath,
                    TipoDocumento = "POLIZA", // Default document type
                    EstadoProcesamiento = processingResult.EstadoProcesamiento,
                    ResultadoJson = JsonConvert.SerializeObject(processingResult.CamposExtraidos),
                    FechaProcesamiento = DateTime.Now,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now
                };

                // Save document to database
                var savedDocument = await _processDocumentRepository.AddAsync(document);

                // Update processing result with document ID
                processingResult.DocumentoId = savedDocument.Id;

                return processingResult;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error processing document: {ex.Message}", ex);
            }
        }
    }
}