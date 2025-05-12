using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IVelneoApiService _velneoApiService;
        private readonly IMapper _mapper;

        public ClientService(
            IClientRepository clientRepository,
            IVelneoApiService velneoApiService,
            IMapper mapper)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _velneoApiService = velneoApiService ?? throw new ArgumentNullException(nameof(velneoApiService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ClientDto> CreateClienteAsync(ClientDto clientDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientDto.Cliruc))
                {
                    var existingClient = await _clientRepository.GetClienteByDocumentoAsync(clientDto.Cliruc);
                    if (existingClient != null)
                    {
                        throw new ApplicationException($"A client with document {clientDto.Cliruc} already exists");
                    }
                }

                // Map DTO to entity
                var clientEntity = _mapper.Map<Client>(clientDto);

                // Set creation date
                clientEntity.FechaCreacion = DateTime.Now;
                clientEntity.FechaModificacion = DateTime.Now;
                clientEntity.Activo = true;

                // Save to database
                var createdClient = await _clientRepository.AddAsync(clientEntity);

                // Return mapped DTO
                return _mapper.Map<ClientDto>(createdClient);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating client: {ex.Message}", ex);
            }
        }

        public async Task DeleteClienteAsync(int id)
        {
            try
            {
                var client = await _clientRepository.GetByIdAsync(id);
                if (client == null)
                {
                    throw new ApplicationException($"Client with ID {id} not found");
                }

                client.Activo = false;
                client.FechaModificacion = DateTime.Now;

                await _clientRepository.UpdateAsync(client);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting client: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientesAsync()
        {
            try
            {
                // Get active clients only
                var clients = await _clientRepository.FindAsync(c => c.Activo);
                return _mapper.Map<IEnumerable<ClientDto>>(clients);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving clients: {ex.Message}", ex);
            }
        }

        public async Task<ClientDto> GetClienteByDocumentoAsync(string documento)
        {
            try
            {
                var client = await _clientRepository.GetClienteByDocumentoAsync(documento);
                return _mapper.Map<ClientDto>(client);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving client by document: {ex.Message}", ex);
            }
        }

        public async Task<ClientDto> GetClienteByEmailAsync(string email)
        {
            try
            {
                var client = await _clientRepository.GetClienteByEmailAsync(email);
                return _mapper.Map<ClientDto>(client);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving client by email: {ex.Message}", ex);
            }
        }

        public async Task<ClientDto> GetClienteByIdAsync(int id)
        {
            try
            {
                var client = await _clientRepository.GetClienteConPolizasAsync(id);

                if (client == null)
                {
                    var clientDto = await _velneoApiService.GetClientAsync(id);
                    if (clientDto != null)
                    {
                        client = _mapper.Map<Client>(clientDto);
                        client.FechaCreacion = DateTime.Now;
                        client.FechaModificacion = DateTime.Now;
                        client.Activo = true;

                        await _clientRepository.AddAsync(client);
                        return clientDto;
                    }
                    return null;
                }

                return _mapper.Map<ClientDto>(client);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving client with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ClientDto>> SearchClientesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllClientesAsync();
                }

                var normalizedSearchTerm = searchTerm.Trim().ToLower();

                var clients = await _clientRepository.FindAsync(c =>
                    c.Activo && (
                        (c.Clinom != null && c.Clinom.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Clirsoc != null && c.Clirsoc.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Cliruc != null && c.Cliruc.ToLower().Contains(normalizedSearchTerm)) ||
                        (c.Cliced != null && c.Cliced.ToLower().Contains(normalizedSearchTerm))
                    )
                );

                return _mapper.Map<IEnumerable<ClientDto>>(clients);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching clients: {ex.Message}", ex);
            }
        }

        public async Task UpdateClienteAsync(ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                {
                    throw new ArgumentNullException(nameof(clientDto));
                }

                var existingClient = await _clientRepository.GetByIdAsync(clientDto.Id);
                if (existingClient == null)
                {
                    throw new ApplicationException($"Client with ID {clientDto.Id} not found");
                }

                var updatedClient = _mapper.Map<Client>(clientDto);
                updatedClient.FechaCreacion = existingClient.FechaCreacion;
                updatedClient.FechaModificacion = DateTime.Now;
                updatedClient.Activo = true;

                await _clientRepository.UpdateAsync(updatedClient);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating client: {ex.Message}", ex);
            }
        }
    }
}