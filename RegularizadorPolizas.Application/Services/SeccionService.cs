using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Services
{
    public class SeccionService : ISeccionService
    {
        private readonly ISeccionRepository _seccionRepository;
        private readonly ILogger<SeccionService> _logger;

        public SeccionService(
            ISeccionRepository seccionRepository,
            ILogger<SeccionService> logger)
        {
            _seccionRepository = seccionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<SeccionDto>> GetAllSeccionesAsync()
        {
            try
            {
                var secciones = await _seccionRepository.GetAllAsync();
                return secciones.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all secciones");
                throw;
            }
        }

        public async Task<IEnumerable<SeccionDto>> GetActiveSeccionesAsync()
        {
            try
            {
                var secciones = await _seccionRepository.GetActiveSeccionesAsync();
                return secciones.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active secciones");
                throw;
            }
        }

        public async Task<IEnumerable<SeccionLookupDto>> GetSeccionesForLookupAsync()
        {
            try
            {
                var secciones = await _seccionRepository.GetActiveSeccionesAsync();
                return secciones.Select(s => new SeccionLookupDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Icono = s.Icono,
                    Activo = s.Activo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting secciones for lookup");
                throw;
            }
        }

        public async Task<SeccionDto?> GetSeccionByIdAsync(int id)
        {
            try
            {
                var seccion = await _seccionRepository.GetByIdAsync(id);
                return seccion != null ? MapToDto(seccion) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seccion by id {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<SeccionDto>> SearchSeccionesAsync(string searchTerm)
        {
            try
            {
                var secciones = await _seccionRepository.SearchSeccionesAsync(searchTerm);
                return secciones.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching secciones with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<SeccionDto> CreateSeccionAsync(CreateSeccionDto createDto)
        {
            try
            {
                // Validar que no exista una sección con el mismo nombre
                if (await _seccionRepository.ExistsAsync(createDto.Name))
                {
                    throw new InvalidOperationException($"Ya existe una sección con el nombre '{createDto.Name}'");
                }

                var seccion = new Seccion
                {
                    Name = createDto.Name.Trim(),
                    Icono = createDto.Icono?.Trim() ?? string.Empty,
                    Activo = createDto.Activo
                };

                var createdSeccion = await _seccionRepository.AddAsync(seccion);
                _logger.LogInformation("Created new seccion {SeccionId} with name {Name}",
                    createdSeccion.Id, createdSeccion.Name);

                return MapToDto(createdSeccion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seccion with name {Name}", createDto.Name);
                throw;
            }
        }

        public async Task<SeccionDto> UpdateSeccionAsync(int id, UpdateSeccionDto updateDto)
        {
            try
            {
                var seccion = await _seccionRepository.GetByIdAsync(id);
                if (seccion == null)
                {
                    throw new KeyNotFoundException($"Seccion with ID {id} not found");
                }

                // Validar que no exista otra sección con el mismo nombre
                if (await _seccionRepository.ExistsAsync(updateDto.Name, id))
                {
                    throw new InvalidOperationException($"Ya existe otra sección con el nombre '{updateDto.Name}'");
                }

                seccion.Name = updateDto.Name.Trim();
                seccion.Icono = updateDto.Icono?.Trim() ?? string.Empty;
                seccion.Activo = updateDto.Activo;

                await _seccionRepository.UpdateAsync(seccion);
                _logger.LogInformation("Updated seccion {SeccionId} with name {Name}",
                    seccion.Id, seccion.Name);

                return MapToDto(seccion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating seccion {Id}", id);
                throw;
            }
        }

        public async Task DeleteSeccionAsync(int id)
        {
            try
            {
                var seccion = await _seccionRepository.GetByIdAsync(id);
                if (seccion == null)
                {
                    throw new KeyNotFoundException($"Seccion with ID {id} not found");
                }

                await _seccionRepository.DeleteAsync(id);
                _logger.LogInformation("Deleted seccion {SeccionId} with name {Name}",
                    id, seccion.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting seccion {Id}", id);
                throw;
            }
        }

        public async Task<bool> SeccionExistsAsync(string name, int? excludeId = null)
        {
            try
            {
                return await _seccionRepository.ExistsAsync(name, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if seccion exists with name {Name}", name);
                throw;
            }
        }

        private static SeccionDto MapToDto(Seccion seccion)
        {
            return new SeccionDto
            {
                Id = seccion.Id,
                Seccion = seccion.Name,
                Icono = seccion.Icono,
                Activo = seccion.Activo,
                FechaCreacion = seccion.FechaCreacion,
                FechaModificacion = seccion.FechaModificacion
            };
        }
    }
}