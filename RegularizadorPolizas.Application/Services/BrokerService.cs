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

        public async Task<BrokerDto> GetBrokerByCodigoAsync(string codigo)
        {
            try
            {
                var broker = await _brokerRepository.GetByCodigoAsync(codigo);
                if (broker == null)
                    return null;

                var brokerDto = _mapper.Map<BrokerDto>(broker);
                brokerDto.TotalPolizas = await GetPolizasCountAsync(broker.Id);

                return brokerDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving broker with codigo {codigo}: {ex.Message}", ex);
            }
        }

        public async Task<BrokerDto> GetBrokerByEmailAsync(string email)
        {
            try
            {
                var broker = await _brokerRepository.GetByEmailAsync(email);
                if (broker == null)
                    return null;

                var brokerDto = _mapper.Map<BrokerDto>(broker);
                brokerDto.TotalPolizas = await GetPolizasCountAsync(broker.Id);

                return brokerDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving broker with email {email}: {ex.Message}", ex);
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
                if (string.IsNullOrWhiteSpace(brokerDto.Nombre))
                    throw new ArgumentException("Broker name is required");

                if (string.IsNullOrWhiteSpace(brokerDto.Codigo))
                    throw new ArgumentException("Broker code is required");

                if (await ExistsByCodigoAsync(brokerDto.Codigo))
                    throw new ArgumentException($"Broker with code '{brokerDto.Codigo}' already exists");

                var broker = _mapper.Map<Broker>(brokerDto);
                broker.Activo = true;

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

                if (await ExistsByCodigoAsync(brokerDto.Codigo, brokerDto.Id))
                    throw new ArgumentException($"Broker with code '{brokerDto.Codigo}' already exists");

                _mapper.Map(brokerDto, existingBroker);
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
                await _brokerRepository.UpdateAsync(broker);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting broker: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                    return false;

                var brokers = await _brokerRepository.FindAsync(b => b.Codigo == codigo);
                return brokers.Any(b => excludeId == null || b.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking broker code existence: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var brokers = await _brokerRepository.FindAsync(b => b.Email == email);
                return brokers.Any(b => excludeId == null || b.Id != excludeId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error checking broker email existence: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BrokerDto>> SearchBrokersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveBrokersAsync();

                var brokers = await _brokerRepository.SearchByNameAsync(searchTerm);
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