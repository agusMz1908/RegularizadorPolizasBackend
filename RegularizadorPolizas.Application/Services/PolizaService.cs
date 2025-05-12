using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Services
{
    public class PolizaService : IPolizaService
    {
        private readonly IPolizaRepository _polizaRepository;
        private readonly IClientRepository _clienteRepository;
        private readonly IRenovacionRepository _renovacionRepository;
        private readonly IMapper _mapper;

        public PolizaService(
            IPolizaRepository polizaRepository,
            IClientRepository clienteRepository,
            IRenovacionRepository renovacionRepository,
            IMapper mapper)
        {
            _polizaRepository = polizaRepository;
            _clienteRepository = clienteRepository;
            _renovacionRepository = renovacionRepository;
            _mapper = mapper;
        }
    }
}