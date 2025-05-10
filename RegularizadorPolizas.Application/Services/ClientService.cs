using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRespository _clientRepository;
        private readonly IMapper _mapper;

        public ClientService(IClientRespository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<ClientDto> CreateClienteAsync(ClientDto clienteDto)
        {
            var cliente = _mapper.Map<Client>(clienteDto);
            var result = await _clientRepository.AddAsync(cliente);
            return _mapper.Map<ClientDto>(result);
        }

        public async Task DeleteClienteAsync(int id)
        {
            await _clientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientesAsync()
        {
            var clientes = await _clientRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClientDto>>(clientes);
        }

        public async Task<ClientDto> GetClienteByIdAsync(int id)
        {
            var cliente = await _clientRepository.GetClienteConPolizasAsync(id);
            return _mapper.Map<ClientDto>(cliente);
        }

        public async Task UpdateClienteAsync(ClientDto clienteDto)
        {
            var cliente = _mapper.Map<Client>(clienteDto);
            await _clientRepository.UpdateAsync(cliente);
        }
    }
}