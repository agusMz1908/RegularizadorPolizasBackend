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
        private readonly IProcessDocumentRepository _processDocumentRepository;
        private readonly IPolizaRepository _polizaRepository;
        private readonly IAzureDocumentIntelligenceService _documentIntelligenceService;
        private readonly IMapper _mapper;

        public ProcessDocumentService(
            IProcessDocumentRepository documentRepository,
            IPolizaRepository polizaRepository,
            IAzureDocumentIntelligenceService documentIntelligenceService,
            IMapper mapper)
        {
            _processDocumentRepository = documentRepository;
            _polizaRepository = polizaRepository;
            _documentIntelligenceService = documentIntelligenceService;
            _mapper = mapper;
        }
    }
}