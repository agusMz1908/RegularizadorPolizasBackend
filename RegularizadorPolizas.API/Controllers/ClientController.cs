using Microsoft.AspNetCore.Mvc;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegularizadorPolizas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            try
            {
                var clients = await _clientService.GetAllClientesAsync();
                if (clients == null || !clients.Any())
                    return NotFound("No clients found");

                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            try
            {
                var client = await _clientService.GetClienteByIdAsync(id);
                if (client == null)
                    return NotFound($"Client with ID {id} not found");

                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClientDto>>> SearchClients([FromQuery] string searchTerm)
        {
            try
            {
                var clients = await _clientService.SearchClientesAsync(searchTerm);
                if (clients == null || !clients.Any())
                    return NotFound($"No clients found matching '{searchTerm}'");

                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("document/{documento}")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClientDto>> GetClientByDocument(string documento)
        {
            try
            {
                var client = await _clientService.GetClienteByDocumentoAsync(documento);
                if (client == null)
                    return NotFound($"Client with document {documento} not found");

                return Ok(client);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClientDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClientDto>> CreateClient(ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                    return BadRequest("Client data is null");

                var createdClient = await _clientService.CreateClienteAsync(clientDto);
                return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateClient(int id, ClientDto clientDto)
        {
            try
            {
                if (clientDto == null)
                    return BadRequest("Client data is null");

                if (id != clientDto.Id)
                    return BadRequest("Client ID mismatch");

                var existingClient = await _clientService.GetClienteByIdAsync(id);
                if (existingClient == null)
                    return NotFound($"Client with ID {id} not found");

                await _clientService.UpdateClienteAsync(clientDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                var existingClient = await _clientService.GetClienteByIdAsync(id);
                if (existingClient == null)
                    return NotFound($"Client with ID {id} not found");

                await _clientService.DeleteClienteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}