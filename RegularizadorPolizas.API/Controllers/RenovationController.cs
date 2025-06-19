using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class RenovationsController : ControllerBase
    {
        private readonly IRenovationService _renovationService;
        private readonly IPolizaService _polizaService;

        public RenovationsController(
            IRenovationService renovationService,
            IPolizaService polizaService)
        {
            _renovationService = renovationService ?? throw new ArgumentNullException(nameof(renovationService));
            _polizaService = polizaService ?? throw new ArgumentNullException(nameof(polizaService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RenovationDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<RenovationDto>>> GetAllRenovations()
        {
            try
            {
                var renovations = await _renovationService.GetAllRenovationsAsync();
                if (renovations == null || !renovations.Any())
                    return NotFound("No renovations found");

                return Ok(renovations);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RenovationDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RenovationDto>> GetRenovationById(int id)
        {
            try
            {
                var renovation = await _renovationService.GetRenovationByIdAsync(id);
                if (renovation == null)
                    return NotFound($"Renovation with ID {id} not found");

                return Ok(renovation);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<RenovationDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<RenovationDto>>> GetRenovationsByStatus(string status)
        {
            try
            {
                var renovations = await _renovationService.GetRenovationsByStatusAsync(status);
                if (renovations == null || !renovations.Any())
                    return NotFound($"No renovations found with status '{status}'");

                return Ok(renovations);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("policy/{polizaId}")]
        [ProducesResponseType(typeof(IEnumerable<RenovationDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<RenovationDto>>> GetRenovationsByPolicy(int polizaId)
        {
            try
            {
                var renovations = await _renovationService.GetRenovationsByPolizaIdAsync(polizaId);
                if (renovations == null || !renovations.Any())
                    return NotFound($"No renovations found for policy with ID {polizaId}");

                return Ok(renovations);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(RenovationDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RenovationDto>> CreateRenovation(RenovationDto renovationDto)
        {
            try
            {
                if (renovationDto == null)
                    return BadRequest("Renovation data is null");

                var createdRenovation = await _renovationService.CreateRenovationAsync(renovationDto);
                return CreatedAtAction(nameof(GetRenovationById), new { id = createdRenovation.Id }, createdRenovation);
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
        public async Task<IActionResult> UpdateRenovation(int id, RenovationDto renovationDto)
        {
            try
            {
                if (renovationDto == null)
                    return BadRequest("Renovation data is null");

                if (id != renovationDto.Id)
                    return BadRequest("Renovation ID mismatch");

                var existingRenovation = await _renovationService.GetRenovationByIdAsync(id);
                if (existingRenovation == null)
                    return NotFound($"Renovation with ID {id} not found");

                await _renovationService.UpdateRenovationAsync(renovationDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/process")]
        [ProducesResponseType(typeof(PolizaDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PolizaDto>> ProcessRenovation(int id)
        {
            try
            {
                var renovation = await _renovationService.GetRenovationByIdAsync(id);
                if (renovation == null)
                    return NotFound($"Renovation with ID {id} not found");

                if (renovation.Estado == "COMPLETADA")
                    return BadRequest("Renovation already processed");

                var renewedPolicy = await _renovationService.ProcessRenovationAsync(id);
                return Ok(renewedPolicy);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CancelRenovation(int id, [FromBody] string reason)
        {
            try
            {
                var renovation = await _renovationService.GetRenovationByIdAsync(id);
                if (renovation == null)
                    return NotFound($"Renovation with ID {id} not found");

                if (renovation.Estado == "COMPLETADA" || renovation.Estado == "CANCELADA")
                    return BadRequest($"Cannot cancel renovation in status '{renovation.Estado}'");

                await _renovationService.CancelRenovationAsync(id, reason);
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