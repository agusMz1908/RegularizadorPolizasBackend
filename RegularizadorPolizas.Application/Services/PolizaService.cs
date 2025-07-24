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

        // ✅ MÉTODO PRINCIPAL PARA CREAR PÓLIZA ENRIQUECIDA (desde frontend)
        public async Task<VelneoSubmissionResult> SubmitPolizaToVelneoAsync(PolizaCreateRequest request, int userId)
        {
            try
            {
                // ✅ VALIDAR REQUEST
                var validation = await ValidatePolizaForVelneoAsync(request);
                if (!validation.IsValid)
                {
                    return new VelneoSubmissionResult
                    {
                        Success = false,
                        Errors = validation.Errors,
                        Message = "Datos no válidos para envío"
                    };
                }

                try
                {
                    // ✅ CONVERTIR PolizaCreateRequest a PolizaDto enriquecido
                    var polizaDto = ConvertirRequestAPolizaDto(request);

                    // ✅ CREAR EN VELNEO
                    var createdPolizaDto = await _velneoApiService.CreatePolizaAsync(polizaDto);

                    if (createdPolizaDto != null)
                    {
                        var velneoId = createdPolizaDto.Id.ToString();
                        await SavePolizaTrackingAsync(polizaDto, userId, velneoId);

                        return new VelneoSubmissionResult
                        {
                            Success = true,
                            VelneoPolizaId = velneoId,
                            Message = "Póliza enriquecida enviada exitosamente a Velneo",
                            LocalTrackingId = createdPolizaDto.Id
                        };
                    }
                    else
                    {
                        return new VelneoSubmissionResult
                        {
                            Success = false,
                            Message = "Velneo retornó respuesta vacía",
                            Errors = new List<string> { "Respuesta vacía de Velneo" }
                        };
                    }
                }
                catch (Exception velneoEx)
                {
                    return new VelneoSubmissionResult
                    {
                        Success = false,
                        Message = $"Error en Velneo: {velneoEx.Message}",
                        Errors = new List<string> { velneoEx.Message }
                    };
                }
            }
            catch (Exception ex)
            {
                return new VelneoSubmissionResult
                {
                    Success = false,
                    Message = $"Error general enviando póliza enriquecida: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
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

        // ✅ MÉTODOS AUXILIARES CORREGIDOS
        
        private async Task<ValidationResult> ValidatePolizaForVelneoAsync(PolizaCreateRequest request)
        {
            var result = new ValidationResult();

            try
            {
                if (string.IsNullOrEmpty(request.Conpol))
                    result.Errors.Add("Número de póliza es obligatorio");

                if (!request.Clinro.HasValue)
                    result.Errors.Add("Cliente es obligatorio");

                if (!request.Comcod.HasValue)
                    result.Errors.Add("Compañía es obligatoria");

                if (string.IsNullOrEmpty(request.Confchdes) || string.IsNullOrEmpty(request.Confchhas))
                    result.Errors.Add("Fechas de vigencia son obligatorias");

                if (!string.IsNullOrEmpty(request.Confchdes) && !string.IsNullOrEmpty(request.Confchhas))
                {
                    if (DateTime.TryParse(request.Confchdes, out var fechaDesde) && 
                        DateTime.TryParse(request.Confchhas, out var fechaHasta))
                    {
                        if (fechaDesde >= fechaHasta)
                            result.Errors.Add("Fecha desde debe ser menor a fecha hasta");
                    }
                }

                if (!string.IsNullOrEmpty(request.Conpol))
                {
                    var existing = await GetPolizaByNumeroAsync(request.Conpol);
                    if (existing != null)
                        result.Warnings.Add($"Ya existe una póliza con número {request.Conpol}");
                }

                if (request.Clinro.HasValue)
                {
                    var client = await _clientRepository.GetByIdAsync(request.Clinro.Value);
                    if (client == null)
                        result.Warnings.Add($"Cliente {request.Clinro} no encontrado en BD local");
                }

                result.IsValid = result.Errors.Count == 0;
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error en validación: {ex.Message}");
                result.IsValid = false;
                return result;
            }
        }

        private PolizaDto ConvertirRequestAPolizaDto(PolizaCreateRequest request)
        {
            return new PolizaDto
            {
                // Campos básicos
                Comcod = request.Comcod,
                Clinro = request.Clinro,
                Conpol = request.Conpol,
                Confchdes = TryParseDate(request.Confchdes),
                Confchhas = TryParseDate(request.Confchhas),
                Conpremio = request.Conpremio,
                
                // Datos del cliente (enriquecidos de Azure AI)
                Clinom = request.Asegurado,
                Condom = request.Direccion,
                
                // Datos del vehículo (enriquecidos de Azure AI)
                Conmaraut = !string.IsNullOrEmpty(request.Vehiculo) ? request.Vehiculo : $"{request.Marca} {request.Modelo}".Trim(),
                Conmotor = request.Motor,
                Conchasis = request.Chasis,
                Conmataut = request.Matricula,
                Conanioaut = TryParseInt(request.Anio?.ToString()),
                
                // Datos financieros
                Moncod = GetMonedaId(request.Moneda),
                Contot = request.PremioTotal,
                
                // Otros datos enriquecidos
                Ramo = request.Ramo ?? "AUTOMOVILES",
                
                // Observaciones enriquecidas con todos los datos de Azure AI
                Observaciones = ConstruirObservacionesEnriquecidas(request),
                
                // Campos de control
                Convig = "1", // Activa
                Consta = "1", // Estado activo  
                Contra = "2", // Tipo de contrato
                Activo = true,
                Procesado = true,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Ingresado = DateTime.Now,
                Last_update = DateTime.Now
            };
        }

        private DateTime? TryParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return null;
            return DateTime.TryParse(dateString, out var result) ? result : null;
        }

        private int? TryParseInt(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return int.TryParse(value, out var result) ? result : null;
        }

        private int GetMonedaId(string moneda)
        {
            return moneda?.ToUpper() switch
            {
                "UYU" => 1,
                "USD" => 2, 
                "EUR" => 3,
                _ => 1 // Default UYU
            };
        }

        private string ConstruirObservacionesEnriquecidas(PolizaCreateRequest request)
        {
            var observaciones = new List<string>();
            
            // Observaciones originales
            if (!string.IsNullOrEmpty(request.Observaciones))
            {
                observaciones.Add(request.Observaciones);
            }
            
            // Información enriquecida de Azure AI
            observaciones.Add("🤖 Procesado automáticamente con Azure Document Intelligence");
            
            if (!string.IsNullOrEmpty(request.Vehiculo))
                observaciones.Add($"🚗 Vehículo: {request.Vehiculo}");
                
            if (!string.IsNullOrEmpty(request.Combustible))
                observaciones.Add($"⛽ Combustible: {request.Combustible}");
                
            if (request.PrimaComercial.HasValue && request.PremioTotal.HasValue)
                observaciones.Add($"💰 Prima: ${request.PrimaComercial:N2} - Premio: ${request.PremioTotal:N2}");
                
            if (!string.IsNullOrEmpty(request.Corredor))
                observaciones.Add($"🏢 Corredor: {request.Corredor}");
                
            // Contacto
            var contacto = new List<string>();
            if (!string.IsNullOrEmpty(request.Email)) contacto.Add($"✉️ {request.Email}");
            if (!string.IsNullOrEmpty(request.Telefono)) contacto.Add($"📞 {request.Telefono}");
            if (contacto.Any())
                observaciones.Add($"📋 Contacto: {string.Join(" | ", contacto)}");
                
            // Ubicación
            var ubicacion = new List<string>();
            if (!string.IsNullOrEmpty(request.Localidad)) ubicacion.Add(request.Localidad);
            if (!string.IsNullOrEmpty(request.Departamento)) ubicacion.Add(request.Departamento);
            if (ubicacion.Any())
                observaciones.Add($"📍 Ubicación: {string.Join(", ", ubicacion)}");
            
            return string.Join(" | ", observaciones);
        }

        private async Task SavePolizaTrackingAsync(PolizaDto polizaDto, int userId, string velneoId)
        {
            var trackingPoliza = _mapper.Map<Poliza>(polizaDto);

            trackingPoliza.TipoOperacion = "NUEVA";
            trackingPoliza.OrigenDocumento = "DOCUMENT_INTELLIGENCE";
            trackingPoliza.Enviado = true;
            trackingPoliza.Procesado = true;
            trackingPoliza.FechaCreacion = DateTime.Now;
            trackingPoliza.FechaModificacion = DateTime.Now;
            trackingPoliza.Activo = true;
            trackingPoliza.Observaciones = $"Velneo ID: {velneoId}, User: {userId} | {polizaDto.Observaciones}";

            await _polizaRepository.AddAsync(trackingPoliza);
        }
    }
}