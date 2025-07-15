using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.Interfaces;
using RegularizadorPolizas.Application.DTOs;
using System.Text.RegularExpressions;


namespace RegularizadorPolizas.Application.Services
{
    public class ClienteMatchingService : IClienteMatchingService
    {
        private readonly IVelneoApiService _velneoService;
        private readonly ILogger<ClienteMatchingService> _logger;

        public ClienteMatchingService(
            IVelneoApiService velneoService,
            ILogger<ClienteMatchingService> logger)
        {
            _velneoService = velneoService;
            _logger = logger;
        }

        public async Task<ClienteMatchResult> BuscarClienteAsync(DatosClienteExtraidos datosExtraidos)
        {
            _logger.LogInformation("🔍 Iniciando búsqueda de cliente: Nombre='{Nombre}', Doc='{Documento}'",
                datosExtraidos.Nombre, datosExtraidos.Documento);

            try
            {
                var resultados = new List<ClienteMatch>();

                // Búsqueda básica por documento o nombre
                if (!string.IsNullOrWhiteSpace(datosExtraidos.Documento))
                {
                    var clientes = await _velneoService.GetClientesAsync();
                    var clientePorDoc = clientes.FirstOrDefault(c =>
                        !string.IsNullOrWhiteSpace(c.Cliruc) &&
                        LimpiarDocumento(c.Cliruc).Equals(LimpiarDocumento(datosExtraidos.Documento), StringComparison.OrdinalIgnoreCase));

                    if (clientePorDoc != null)
                    {
                        resultados.Add(new ClienteMatch
                        {
                            Cliente = clientePorDoc,
                            Score = 100,
                            Criterio = "Documento exacto",
                            Coincidencias = new List<string> { "Documento coincidente" }
                        });
                    }
                }

                return CrearResultado(resultados, datosExtraidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda de cliente");

                return new ClienteMatchResult
                {
                    TipoResultado = ClienteMatchResult.TipoResultadoCliente.SinCoincidencias,
                    MensajeUsuario = "Error en la búsqueda. Intente búsqueda manual.",
                    RequiereIntervencionManual = true,
                    DatosOriginales = datosExtraidos
                };
            }
        }

        public async Task<ClienteMatchResult> BuscarClientePorDocumentoAsync(string documento)
        {
            var datos = new DatosClienteExtraidos { Documento = documento };
            return await BuscarClienteAsync(datos);
        }

        public async Task<ClienteMatchResult> BuscarClientePorNombreAsync(string nombre)
        {
            var datos = new DatosClienteExtraidos { Nombre = nombre };
            return await BuscarClienteAsync(datos);
        }

        public async Task<List<ClienteMatch>> BuscarClientesAvanzadoAsync(DatosClienteExtraidos datos, int limiteSugerencias = 10)
        {
            var resultado = await BuscarClienteAsync(datos);
            return resultado.Matches.Take(limiteSugerencias).ToList();
        }

        #region Métodos Privados

        private ClienteMatchResult CrearResultado(List<ClienteMatch> matches, DatosClienteExtraidos datosOriginales)
        {
            var resultado = new ClienteMatchResult
            {
                Matches = matches,
                DatosOriginales = datosOriginales
            };

            if (!matches.Any())
            {
                resultado.TipoResultado = ClienteMatchResult.TipoResultadoCliente.SinCoincidencias;
                resultado.MensajeUsuario = "No se encontraron clientes coincidentes.";
                resultado.RequiereIntervencionManual = true;
            }
            else if (matches.Count == 1 && matches[0].Score >= 95)
            {
                resultado.TipoResultado = ClienteMatchResult.TipoResultadoCliente.MatchExacto;
                resultado.MensajeUsuario = $"Cliente encontrado: {matches[0].Cliente.Clinom}";
                resultado.RequiereIntervencionManual = false;
            }
            else
            {
                resultado.TipoResultado = ClienteMatchResult.TipoResultadoCliente.MultiplesMatches;
                resultado.MensajeUsuario = "Se encontraron coincidencias. Revise.";
                resultado.RequiereIntervencionManual = true;
            }

            return resultado;
        }

        private string LimpiarDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento)) return "";
            return Regex.Replace(documento, @"[^\d]", "").TrimStart('0');
        }

        #endregion
    }
}