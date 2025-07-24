using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class PolizaService : IPolizaService
    {
        private readonly IPolizaRepository _polizaRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IRenovationRepository _renovationRepository;
        private readonly IVelneoApiService _velneoApiService;
        private readonly IMapper _mapper;

        public PolizaService(
            IPolizaRepository polizaRepository,
            IClientRepository clientRepository,
            IRenovationRepository renovacionRepository,
            IVelneoApiService velneoApiService,
            IMapper mapper)
        {
            _polizaRepository = polizaRepository ?? throw new ArgumentNullException(nameof(polizaRepository));
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _renovationRepository = renovacionRepository ?? throw new ArgumentNullException(nameof(renovacionRepository));
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // ✅ MÉTODO PARA CREAR PÓLIZA DIRECTA (uso interno)
        public async Task<PolizaDto> CreatePolizaAsync(PolizaDto polizaDto)
        {
            try
            {
                // Validate client exists
                if (polizaDto.Clinro.HasValue)
                {
                    var client = await _clientRepository.GetByIdAsync(polizaDto.Clinro.Value);
                    if (client == null)
                    {
                        var clientDto = await _velneoApiService.GetClienteAsync(polizaDto.Clinro.Value);
                        if (clientDto != null)
                        {
                            var newClient = _mapper.Map<Client>(clientDto);
                            newClient.FechaCreacion = DateTime.Now;
                            newClient.FechaModificacion = DateTime.Now;
                            newClient.Activo = true;
                            await _clientRepository.AddAsync(newClient);
                        }
                        else
                        {
                            throw new ApplicationException($"Client with ID {polizaDto.Clinro} not found");
                        }
                    }
                }

                var poliza = _mapper.Map<Poliza>(polizaDto);

                poliza.FechaCreacion = DateTime.Now;
                poliza.FechaModificacion = DateTime.Now;
                poliza.Activo = true;

                var createdPoliza = await _polizaRepository.AddAsync(poliza);

                return _mapper.Map<PolizaDto>(createdPoliza);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating policy: {ex.Message}", ex);
            }
        }

        // ✅ OTROS MÉTODOS (mantenidos como están)
        public async Task DeletePolizaAsync(int id)
        {
            try
            {
                var poliza = await _polizaRepository.GetByIdAsync(id);
                if (poliza == null)
                {
                    throw new ApplicationException($"Policy with ID {id} not found");
                }

                poliza.Activo = false;
                poliza.FechaModificacion = DateTime.Now;

                await _polizaRepository.UpdateAsync(poliza);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting policy: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetAllPolizasAsync()
        {
            try
            {
                var polizas = await _polizaRepository.FindAsync(p => p.Activo);
                return _mapper.Map<IEnumerable<PolizaDto>>(polizas);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving policies: {ex.Message}", ex);
            }
        }

        public async Task<PolizaDto> GetPolizaByIdAsync(int id)
        {
            try
            {
                var poliza = await _polizaRepository.GetPolizaDetalladaAsync(id);

                if (poliza == null)
                {
                    var polizaDto = await _velneoApiService.GetPolizaAsync(id);
                    if (polizaDto != null)
                    {
                        if (polizaDto.Clinro.HasValue)
                        {
                            var client = await _clientRepository.GetByIdAsync(polizaDto.Clinro.Value);
                            if (client == null)
                            {
                                var clientDto = await _velneoApiService.GetClienteAsync(polizaDto.Clinro.Value);
                                if (clientDto != null)
                                {
                                    var newClient = _mapper.Map<Client>(clientDto);
                                    newClient.FechaCreacion = DateTime.Now;
                                    newClient.FechaModificacion = DateTime.Now;
                                    newClient.Activo = true;
                                    await _clientRepository.AddAsync(newClient);
                                }
                            }
                        }

                        poliza = _mapper.Map<Poliza>(polizaDto);
                        poliza.FechaCreacion = DateTime.Now;
                        poliza.FechaModificacion = DateTime.Now;
                        poliza.Activo = true;

                        await _polizaRepository.AddAsync(poliza);
                        return polizaDto;
                    }
                    return null;
                }

                return _mapper.Map<PolizaDto>(poliza);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving policy with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<PolizaDto>> GetPolizasByClienteAsync(int clienteId)
        {
            try
            {
                var polizas = await _polizaRepository.GetPolizasByClienteAsync(clienteId);
                return _mapper.Map<IEnumerable<PolizaDto>>(polizas);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving policies for client {clienteId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<PolizaDto>> SearchPolizasAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllPolizasAsync();
                }
                var normalizedSearchTerm = searchTerm.Trim().ToLower();

                var polizas = await _polizaRepository.FindAsync(p =>
                    p.Activo && (
                        (p.Conpol != null && p.Conpol.ToLower().Contains(normalizedSearchTerm)) ||
                        (p.Conmaraut != null && p.Conmaraut.ToLower().Contains(normalizedSearchTerm)) ||
                        (p.Conmataut != null && p.Conmataut.ToLower().Contains(normalizedSearchTerm)) ||
                        (p.Conmotor != null && p.Conmotor.ToLower().Contains(normalizedSearchTerm)) ||
                        (p.Conchasis != null && p.Conchasis.ToLower().Contains(normalizedSearchTerm)) ||
                        (p.Ramo != null && p.Ramo.ToLower().Contains(normalizedSearchTerm))
                    )
                );

                return _mapper.Map<IEnumerable<PolizaDto>>(polizas);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching policies: {ex.Message}", ex);
            }
        }

        public async Task UpdatePolizaAsync(PolizaDto polizaDto)
        {
            try
            {
                if (polizaDto == null)
                {
                    throw new ArgumentNullException(nameof(polizaDto));
                }

                var existingPoliza = await _polizaRepository.GetByIdAsync(polizaDto.Id);
                if (existingPoliza == null)
                {
                    throw new ApplicationException($"Policy with ID {polizaDto.Id} not found");
                }

                if (polizaDto.Clinro.HasValue && polizaDto.Clinro.Value != existingPoliza.Clinro)
                {
                    var client = await _clientRepository.GetByIdAsync(polizaDto.Clinro.Value);
                    if (client == null)
                    {
                        throw new ApplicationException($"Client with ID {polizaDto.Clinro} not found");
                    }
                }

                var updatedPoliza = _mapper.Map<Poliza>(polizaDto);
                updatedPoliza.FechaCreacion = existingPoliza.FechaCreacion;
                updatedPoliza.FechaModificacion = DateTime.Now;
                updatedPoliza.Activo = true;

                await _polizaRepository.UpdateAsync(updatedPoliza);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating policy: {ex.Message}", ex);
            }
        }

        public async Task<PolizaDto> GetPolizaByNumeroAsync(string numeroPoliza)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroPoliza))
                    return null;

                var poliza = await _polizaRepository.GetPolizaByNumeroAsync(numeroPoliza);
                return poliza != null ? _mapper.Map<PolizaDto>(poliza) : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error buscando póliza {numeroPoliza}: {ex.Message}", ex);
            }
        }
    }
}