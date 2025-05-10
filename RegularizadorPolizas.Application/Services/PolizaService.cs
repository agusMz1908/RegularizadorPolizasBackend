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
        private readonly IClientRespository _clienteRepository;
        private readonly IRenovacionRepository _renovacionRepository;
        private readonly IMapper _mapper;

        public PolizaService(
            IPolizaRepository polizaRepository,
            IClientRespository clienteRepository,
            IRenovacionRepository renovacionRepository,
            IMapper mapper)
        {
            _polizaRepository = polizaRepository;
            _clienteRepository = clienteRepository;
            _renovacionRepository = renovacionRepository;
            _mapper = mapper;
        }

        public async Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto)
        {
            var poliza = _mapper.Map<Poliza>(polizaDto);
            var result = await _polizaRepository.AddAsync(poliza);
            return _mapper.Map<PolizaDto>(result);
        }

        public async Task DeletePolizaAsync(int id)
        {
            await _polizaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PolizaDto>> GetAllPolizasAsync()
        {
            var polizas = await _polizaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PolizaDto>>(polizas);
        }

        public async Task<PolizaDto> GetPolizaByIdAsync(int id)
        {
            var poliza = await _polizaRepository.GetPolizaDetalladaAsync(id);
            return _mapper.Map<PolizaDto>(poliza);
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasByClienteAsync(int clienteId)
        {
            var polizas = await _polizaRepository.GetPolizasByClienteAsync(clienteId);
            return _mapper.Map<IEnumerable<PolizaDto>>(polizas);
        }

        public async Task<PolizaDto> RenovarPolizaAsync(int polizaId, RenovationDto renovacionDto)
        {
            // Obtener la póliza original
            var polizaOriginal = await _polizaRepository.GetPolizaDetalladaAsync(polizaId);
            if (polizaOriginal == null)
                throw new ApplicationException("Póliza no encontrada");

            // Crear la nueva póliza con los datos de la original
            var nuevaPoliza = new Poliza
            {
                Clinro = polizaOriginal.Clinro,
                Comcod = polizaOriginal.Comcod,
                Seccod = polizaOriginal.Seccod,
                Conpol = $"{polizaOriginal.Conpol}-R", // Sufijo para indicar renovación
                Conmaraut = polizaOriginal.Conmaraut,
                Conanioaut = polizaOriginal.Conanioaut,
                Conmataut = polizaOriginal.Conmataut,
                Conmotor = polizaOriginal.Conmotor,
                Conpadaut = polizaOriginal.Conpadaut,
                Conchasis = polizaOriginal.Conchasis,
                Conpremio = polizaOriginal.Conpremio,
                Moncod = polizaOriginal.Moncod,
                Concuo = polizaOriginal.Concuo,
                Concomcorr = polizaOriginal.Concomcorr,
                Conpadre = polizaOriginal.Id, // Referencia a la póliza original
                Ramo = polizaOriginal.Ramo,
                ComAlias = polizaOriginal.ComAlias,
                Convig = "1", // Estado activo
                // Configurar fechas de vigencia
                Confchdes = DateTime.Now,
                Confchhas = DateTime.Now.AddYears(1), // Un año de vigencia por defecto
                Observaciones = renovacionDto.Observaciones,
                Activo = true
                // Agregar otros campos según sea necesario
            };

            // Guardar la nueva póliza
            var polizaCreada = await _polizaRepository.AddAsync(nuevaPoliza);

            // Registrar la renovación
            var renovacion = new Renovacion
            {
                PolizaId = polizaId,
                PolizaNuevaId = polizaCreada.Id,
                FechaSolicitud = DateTime.Now,
                Estado = "COMPLETADA",
                Observaciones = renovacionDto.Observaciones,
                UsuarioId = renovacionDto.UsuarioId
            };

            await _renovacionRepository.AddAsync(renovacion);

            return _mapper.Map<PolizaDto>(polizaCreada);
        }

        public async Task UpdatePolizaAsync(PolizaDto polizaDto)
        {
            var poliza = _mapper.Map<Poliza>(polizaDto);
            await _polizaRepository.UpdateAsync(poliza);
        }
    }
}