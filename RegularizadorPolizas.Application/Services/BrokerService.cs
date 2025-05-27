using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IBrokerRepository _brokerRepository;
        private readonly IPolizaRepository _polizaRepository;
        private readonly IMapper _mapper;

        public BrokerService(
            IBrokerRepository brokerRepository,
            IPolizaRepository polizaRepository,
            IMapper mapper)
        {
            _brokerRepository = brokerRepository ?? throw new ArgumentNullException(nameof(brokerRepository));
            _polizaRepository = polizaRepository ?? throw new ArgumentNullException(nameof(polizaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<BrokerDto>> GetAllBrokersAsync()
        {
            try
            {
                var brokers = await _brokerRepository.GetAllAsync();
                var brokersDto = _mapper.Map<IEnumerable<BrokerDto>>(brokers);

                foreach (var brokerDto in brokersDto)
                {
                    var polizasCount = await GetPolizasCountAsync(brokerDto.Id);
                    brokerDto.TotalPolizas = polizasCount;
                }

                return brokersDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving brokers: {ex.Message}", ex);
            }
        }

        public async Task<BrokerDto> GetBrokerByIdAsync(int id)
        {
            try
            {
                var broker = await _brokerRepository.GetByIdAsync(id);
                if (broker == null)
                    return null;

                var brokerDto = _mapper.Map<BrokerDto>(broker);
                brokerDto.TotalPolizas = await GetPolizasCountAsync(id);

                return brokerDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving broker with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<BrokerDto> GetBrokerByNameAsync(string name)
        {
            try
            {
                var broker = await _brokerRepository.GetByNameAsync(name);
                if (broker == null)
                    return null;

                var brokerDto = _mapper.Map<BrokerDto>(broker);
                brokerDto.TotalPolizas = await GetPolizasCountAsync(broker.Id);

                return brokerDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving broker with name {name}: {ex.Message}", ex);
            }
        }

        public async Task<BrokerDto> GetBrokerByTelefonoAsync(string telefono)
        {
            try
            {
                var broker = await _brokerRepository.GetByTelefonoAsync(telefono);
                if (broker == null)
                    return null;

                var brokerDto = _mapper.Map<BrokerDto>(broker);
                brokerDto.TotalPolizas = await GetPolizasCountAsync(broker.Id);

                return brokerDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving broker with phone {telefono}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BrokerDto>> GetActiveBrokersAsync()
        {
            try
            {
                var brokers = await _brokerRepository.GetActiveBrokersAsync();
                return _mapper.Map<IEnumerable<BrokerDto>>(brokers);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving active brokers: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BrokerLookupDto>> GetBrokersForLookupAsync()
        {
            try
            {
                var brokers = await _brokerRepository.GetActiveBrokersAsync();
                return _mapper.Map<IEnumerable<BrokerLookupDto>>(brokers);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving brokers for lookup: {ex.Message}", ex);
            }
        }

        public async Task<BrokerDto> CreateBrokerAsync(BrokerDto brokerDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brokerDto.Name))
                    throw new ArgumentException("Broker name is required");

                if (await ExistsByNameAsync(brokerDto.Name))
                    throw new ArgumentException($"Broker with name '{brokerDto.Name}' already exists");

                if (!string.IsNullOrWhiteSpace(brokerDto.Telefono))
                {
                    if (await ExistsByTelefonoAsync(brokerDto.Telefono))
                        throw new ArgumentException($"Broker with phone '{brokerDto.Telefono}' already exists");
                }

                var broker = _mapper.Map<Broker>(brokerDto);
                broker.Activo = true;
                broker.FechaCreacion = DateTime.Now;
                broker.FechaModificacion = DateTime.Now;

                var createdBroker = await _brokerRepository.AddAsync(broker);
                return _mapper.Map<BrokerDto>(createdBroker);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating broker: {ex.Message}", ex);
            }
        }

        public async Task UpdateBrokerAsync(BrokerDto brokerDto)
        {
            try
            {
                if (brokerDto == null)
                    throw new ArgumentNullException(nameof(brokerDto));

                var existingBroker = await _brokerRepository.GetByIdAsync(brokerDto.Id);
                if (existingBroker == null)
                    throw new ApplicationException($"Broker with ID {brokerDto.Id} not found");

                if (await ExistsByNameAsync(brokerDto.Name, brokerDto.Id))
                    throw new ArgumentException($"Broker with name '{brokerDto.Name}' already exists");

                if (!string.IsNullOrWhiteSpace(brokerDto.Telefono) &&
                    await ExistsByTelefonoAsync(brokerDto.Telefono, brokerDto.Id))
                    throw new ArgumentException($"Broker with phone '{brokerDto.Telefono}' already exists");

                // Actualizar propiedades manualmente para mantener control
                existingBroker.Name = brokerDto.Name;
                existingBroker.Telefono = brokerDto.Telefono;
                existingBroker.Direccion = brokerDto.Direccion;
                existingBroker.Observaciones = brokerDto.Observaciones;
                existingBroker.Activo = brokerDto.Activo;
                existingBroker.FechaModificacion = DateTime.Now;

                // Manejar foto si se proporciona
                if (!string.IsNullOrEmpty(brokerDto.Foto))
                {
                    try
                    {
                        existingBroker.Foto = Convert.FromBase64String(brokerDto.Foto);
                    }
                    catch
                    {
                        throw new ArgumentException("Invalid image format. Please provide a valid base64 string.");
                    }
                }

                await _brokerRepository.UpdateAsync(existingBroker);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating broker: {ex.Message}", ex);
            }
        }

        public async Task DeleteBrokerAsync(int id)
        {
            try
            {
                var broker = await _brokerRepository.GetByIdAsync(id);
                if (broker == null)
                    throw new ApplicationException($"Broker with ID {id} not found");

                var polizasCount = await GetPolizasCountAsync(id);
                if (polizasCount > 0)
                    throw new ApplicationException($"Cannot delete broker. It has {polizasCount} associated policies");

                broker.Activo = false;
                broker.FechaModificacion = DateTime.Now;
                await _brokerRepository.UpdateAsync(broker);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting broker: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return false;

                var brokers = await _brokerRepository.FindAsync(b => b.Name.ToLower() == name.ToLower());
                return brokers.Any(b => excludeId == null || b.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking broker name existence: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByTelefonoAsync(string telefono, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(telefono))
                    return false;

                var brokers = await _brokerRepository.FindAsync(b => b.Telefono == telefono);
                return brokers.Any(b => excludeId == null || b.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking broker phone existence: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveBrokersAsync();

                var normalizedSearchTerm = searchTerm.Trim().ToLower();
                var brokers = await _brokerRepository.FindAsync(b =>
                    b.Activo && (
                        b.Name.ToLower().Contains(normalizedSearchTerm) ||
                        (b.Telefono != null && b.Telefono.Contains(normalizedSearchTerm)) ||
                        (b.Direccion != null && b.Direccion.ToLower().Contains(normalizedSearchTerm))
                    )
                );

                return _mapper.Map<IEnumerable<BrokerDto>>(brokers);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching brokers: {ex.Message}", ex);
            }
        }

        private async Task<int> GetPolizasCountAsync(int brokerId)
        {
            try
            {
                var polizas = await _polizaRepository.FindAsync(p => p.Corrnom == brokerId && p.Activo);
                return polizas.Count();
            }
            catch
            {
                return 0;
            }
        }
    }
}