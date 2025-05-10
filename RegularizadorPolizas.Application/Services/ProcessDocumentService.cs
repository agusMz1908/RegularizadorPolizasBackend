using AutoMapper;
using Microsoft.AspNetCore.Http;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Services
{
    public class ProcessDocumentService : IProcessDocumentService
    {
        private readonly IProcessDocumentService _processDocumentRepository;
        private readonly IPolizaRepository _polizaRepository;
        private readonly IAzureDocumentIntelligenceService _documentIntelligenceService;
        private readonly IMapper _mapper;

        public ProcessDocumentService(
            IProcessDocumentService documentoRepository,
            IPolizaRepository polizaRepository,
            IAzureDocumentIntelligenceService documentIntelligenceService,
            IMapper mapper)
        {
            _processDocumentRepository = documentoRepository;
            _polizaRepository = polizaRepository;
            _documentIntelligenceService = documentIntelligenceService;
            _mapper = mapper;
        }

        public async Task<PolizaDto> ExtractPolizaFromDocumentAsync(int documentoId)
        {
            // Obtener el documento procesado
            var documento = await _processDocumentRepository.GetByIdAsync(documentoId);
            if (documento == null)
                throw new ApplicationException("Documento no encontrado");

            if (documento.EstadoProcesamiento != "PROCESADO")
                throw new ApplicationException("El documento no ha sido procesado correctamente");

            // Crear un DocumentoResultadoDto a partir del documento procesado
            var documentoResultado = new DocumentResutDto
            {
                DocumentoId = documento.Id,
                NombreArchivo = documento.NombreArchivo,
                EstadoProcesamiento = documento.EstadoProcesamiento,
                CamposExtraidos = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(documento.ResultadoJson)
            };

            // Usar el servicio de Document Intelligence para mapear los campos a una póliza
            var polizaDto = _documentIntelligenceService.MapDocumentToPoliza(documentoResultado);

            return polizaDto;
        }

        public async Task<DocumentResutDto> GetDocumentProcessingResultAsync(int documentoId)
        {
            var documento = await _processDocumentRepository.GetDocumentoConPolizaAsync(documentoId);
            if (documento == null)
                return null;

            var resultado = new DocumentResutDto
            {
                DocumentoId = documento.Id,
                NombreArchivo = documento.NombreArchivo,
                EstadoProcesamiento = documento.EstadoProcesamiento,
                CamposExtraidos = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(documento.ResultadoJson),
                RequiereRevision = documento.EstadoProcesamiento == "REQUIERE_REVISION"
            };

            return resultado;
        }

        public async Task<DocumentResutDto> ProcessDocumentAsync(IFormFile file)
        {
            // Procesar el documento con Azure Document Intelligence
            var resultado = await _documentIntelligenceService.ProcessDocumentAsync(file);

            // Guardar el resultado en la base de datos
            var documento = new DocumentResutDto
            {
                NombreArchivo = file.FileName,
                RutaArchivo = $"/documentos/{Guid.NewGuid()}-{file.FileName}", // Ruta donde se guardará el archivo
                TipoDocumento = "POLIZA", // Por defecto asumimos que es una póliza
                EstadoProcesamiento = resultado.EstadoProcesamiento,
                ResultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado.CamposExtraidos),
                FechaProcesamiento = DateTime.Now
            };

            // Guarda el documento en la base de datos
            var documentoGuardado = await _processDocumentRepository.AddAsync(documento);

            // Actualiza el ID del documento en el resultado
            resultado.DocumentoId = documentoGuardado.Id;

            return resultado;
        }
    }
}