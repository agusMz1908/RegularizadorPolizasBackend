using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Services
{
    public class RenovationService : IRenovationService
    {
        private readonly IRenovationRepository _renovationRepository;
        private readonly IPolizaRepository _polizaRepository;
        private readonly IMapper _mapper;

        public RenovationService(
            IRenovationRepository renovationRepository,
            IPolizaRepository polizaRepository,
            IMapper mapper)
        {
            _renovationRepository = renovationRepository ?? throw new ArgumentNullException(nameof(renovationRepository));
            _polizaRepository = polizaRepository ?? throw new ArgumentNullException(nameof(polizaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task CancelRenovationAsync(int renovationId, string reason)
        {
            try
            {
                var renovation = await _renovationRepository.GetByIdAsync(renovationId);
                if (renovation == null)
                {
                    throw new ApplicationException($"Renovation with ID {renovationId} not found");
                }

                if (renovation.Estado == "COMPLETADA" || renovation.Estado == "CANCELADA")
                {
                    throw new ApplicationException($"Cannot cancel renovation in status '{renovation.Estado}'");
                }

                renovation.Estado = "CANCELADA";
                renovation.Observaciones = string.IsNullOrEmpty(renovation.Observaciones)
                    ? $"Cancelada: {reason}"
                    : $"{renovation.Observaciones}\nCancelada: {reason}";
                renovation.FechaModificacion = DateTime.Now;

                await _renovationRepository.UpdateAsync(renovation);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error canceling renovation: {ex.Message}", ex);
            }
        }

        public async Task<RenovationDto> CreateRenovationAsync(RenovationDto renovationDto)
        {
            try
            {
                // Validate original policy exists
                var originalPolicy = await _polizaRepository.GetByIdAsync(renovationDto.PolizaId);
                if (originalPolicy == null)
                {
                    throw new ApplicationException($"Original policy with ID {renovationDto.PolizaId} not found");
                }

                // Create new renovation entity
                var renovation = _mapper.Map<Renovation>(renovationDto);
                renovation.Estado = "PENDIENTE"; // Default status
                renovation.FechaSolicitud = DateTime.Now;
                renovation.FechaCreacion = DateTime.Now;
                renovation.FechaModificacion = DateTime.Now;

                // Save to database
                var createdRenovation = await _renovationRepository.AddAsync(renovation);

                // Return mapped DTO
                return _mapper.Map<RenovationDto>(createdRenovation);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error creating renovation: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<RenovationDto>> GetAllRenovationsAsync()
        {
            try
            {
                var renovations = await _renovationRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<RenovationDto>>(renovations);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error retrieving renovations: {ex.Message}", ex);
            }
        }

        public async Task<RenovationDto> GetRenovationByIdAsync(int id)
        {
            try
            {
                var renovation = await _renovationRepository.GetRenovationWithDetailsAsync(id);
                return _mapper.Map<RenovationDto>(renovation);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error retrieving renovation with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<RenovationDto>> GetRenovationsByPolizaIdAsync(int polizaId)
        {
            try
            {
                var renovations = await _renovationRepository.GetRenovationsByPolicyAsync(polizaId);
                return _mapper.Map<IEnumerable<RenovationDto>>(renovations);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error retrieving renovations for policy {polizaId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<RenovationDto>> GetRenovationsByStatusAsync(string status)
        {
            try
            {
                var renovations = await _renovationRepository.GetRenovationsByStatusAsync(status);
                return _mapper.Map<IEnumerable<RenovationDto>>(renovations);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error retrieving renovations with status {status}: {ex.Message}", ex);
            }
        }

        public async Task<PolizaDto> ProcessRenovationAsync(int renovationId)
        {
            try
            {
                // Get renovation with details
                var renovation = await _renovationRepository.GetRenovationWithDetailsAsync(renovationId);
                if (renovation == null)
                {
                    throw new ApplicationException($"Renovation with ID {renovationId} not found");
                }

                if (renovation.Estado == "COMPLETADA")
                {
                    throw new ApplicationException("Renovation already processed");
                }

                if (renovation.Estado == "CANCELADA")
                {
                    throw new ApplicationException("Cannot process canceled renovation");
                }

                // Get original policy
                var originalPolicy = await _polizaRepository.GetPolizaDetalladaAsync(renovation.PolizaId);
                if (originalPolicy == null)
                {
                    throw new ApplicationException($"Original policy with ID {renovation.PolizaId} not found");
                }

                // Create a new policy based on the original one
                var nuevaPoliza = new Poliza
                {
                    Clinro = originalPolicy.Clinro,
                    Comcod = originalPolicy.Comcod,
                    Seccod = originalPolicy.Seccod,
                    Conpol = $"{originalPolicy.Conpol}-R", // Add suffix to indicate renewal
                    Conmaraut = originalPolicy.Conmaraut,
                    Conanioaut = originalPolicy.Conanioaut,
                    Conmataut = originalPolicy.Conmataut,
                    Conmotor = originalPolicy.Conmotor,
                    Conpadaut = originalPolicy.Conpadaut,
                    Conchasis = originalPolicy.Conchasis,
                    Conpremio = originalPolicy.Conpremio,
                    Moncod = originalPolicy.Moncod,
                    Concuo = originalPolicy.Concuo,
                    Concomcorr = originalPolicy.Concomcorr,
                    Conpadre = originalPolicy.Id, // Reference to original policy
                    Ramo = originalPolicy.Ramo,
                    Com_alias = originalPolicy.Com_alias,
                    Convig = "1", // Active status
                    // Set new validity dates
                    Confchdes = DateTime.Now,
                    Confchhas = DateTime.Now.AddYears(1), // Default 1 year validity
                    Observaciones = renovation.Observaciones,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now
                };

                // Save the new policy
                var polizaCreada = await _polizaRepository.AddAsync(nuevaPoliza);

                // Update renovation information
                renovation.PolizaNuevaId = polizaCreada.Id;
                renovation.Estado = "COMPLETADA";
                renovation.FechaModificacion = DateTime.Now;

                await _renovationRepository.UpdateAsync(renovation);

                // Return the created policy DTO
                return _mapper.Map<PolizaDto>(polizaCreada);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error processing renovation: {ex.Message}", ex);
            }
        }

        public async Task UpdateRenovationAsync(RenovationDto renovationDto)
        {
            try
            {
                if (renovationDto == null)
                {
                    throw new ArgumentNullException(nameof(renovationDto));
                }

                // Get existing renovation
                var existingRenovation = await _renovationRepository.GetByIdAsync(renovationDto.Id);
                if (existingRenovation == null)
                {
                    throw new ApplicationException($"Renovation with ID {renovationDto.Id} not found");
                }

                // Don't allow changes if already completed or canceled
                if (existingRenovation.Estado == "COMPLETADA" || existingRenovation.Estado == "CANCELADA")
                {
                    throw new ApplicationException($"Cannot update renovation in status '{existingRenovation.Estado}'");
                }

                // Update properties but preserve some
                var updatedRenovation = _mapper.Map<Renovation>(renovationDto);
                updatedRenovation.FechaCreacion = existingRenovation.FechaCreacion;
                updatedRenovation.FechaSolicitud = existingRenovation.FechaSolicitud;
                updatedRenovation.FechaModificacion = DateTime.Now;

                await _renovationRepository.UpdateAsync(updatedRenovation);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException($"Error updating renovation: {ex.Message}", ex);
            }
        }
    }
}