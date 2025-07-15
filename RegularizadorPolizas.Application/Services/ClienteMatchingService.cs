using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System.Text.RegularExpressions;
using static ClienteMatchResult;

namespace RegularizadorPolizas.Application.Services
{
    public interface IClienteMatchingService
    {
        Task<ClienteMatchResult> BuscarClienteAsync(DatosClienteExtraidos datosExtraidos);
        Task<ClienteMatchResult> BuscarClientePorDocumentoAsync(string documento);
        Task<ClienteMatchResult> BuscarClientePorNombreAsync(string nombre);
        Task<List<ClienteMatch>> BuscarClientesAvanzadoAsync(DatosClienteExtraidos datos, int limiteSugerencias = 10);
    }

    public class ClienteMatchingService : IClienteMatchingService
    {
        private readonly ITenantAwareVelneoApiService _velneoService;
        private readonly ILogger<ClienteMatchingService> _logger;

        public ClienteMatchingService(
            ITenantAwareVelneoApiService velneoService,
            ILogger<ClienteMatchingService> logger)
        {
            _velneoService = velneoService;
            _logger = logger;
        }

        public async Task<ClienteMatchResult> BuscarClienteAsync(DatosClienteExtraidos datosExtraidos)
        {
            _logger.LogInformation("🔍 Iniciando búsqueda de cliente: Nombre='{Nombre}', Doc='{Documento}'",
                datosExtraidos.Nombre, datosExtraidos.Documento);

            var resultados = new List<ClienteMatch>();

            try
            {
                // 1. Búsqueda por documento (más confiable)
                if (!string.IsNullOrWhiteSpace(datosExtraidos.Documento))
                {
                    var matchesPorDocumento = await BuscarPorDocumento(datosExtraidos.Documento, datosExtraidos);
                    resultados.AddRange(matchesPorDocumento);
                }

                // 2. Búsqueda por nombre (si no encontramos match exacto por documento)
                if (!resultados.Any(m => m.Score >= 95) && !string.IsNullOrWhiteSpace(datosExtraidos.Nombre))
                {
                    var matchesPorNombre = await BuscarPorNombre(datosExtraidos.Nombre, datosExtraidos);

                    // Evitar duplicados
                    foreach (var match in matchesPorNombre)
                    {
                        if (!resultados.Any(r => r.Cliente.Id == match.Cliente.Id))
                        {
                            resultados.Add(match);
                        }
                    }
                }

                // 3. Búsqueda por email (si tenemos email y no hay matches buenos)
                if (!resultados.Any(m => m.Score >= 80) && !string.IsNullOrWhiteSpace(datosExtraidos.Email))
                {
                    var matchesPorEmail = await BuscarPorEmail(datosExtraidos.Email, datosExtraidos);

                    foreach (var match in matchesPorEmail)
                    {
                        if (!resultados.Any(r => r.Cliente.Id == match.Cliente.Id))
                        {
                            resultados.Add(match);
                        }
                    }
                }

                // Ordenar por score y tomar los mejores
                resultados = resultados.OrderByDescending(x => x.Score).Take(10).ToList();

                return CrearResultado(resultados, datosExtraidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en búsqueda de cliente");

                return new ClienteMatchResult
                {
                    TipoResultado = TipoResultadoCliente.SinCoincidencias,
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

        #region Métodos de Búsqueda Específicos

        private async Task<List<ClienteMatch>> BuscarPorDocumento(string documento, DatosClienteExtraidos datosOriginales)
        {
            var matches = new List<ClienteMatch>();

            try
            {
                // Limpiar y normalizar documento
                var docLimpio = LimpiarDocumento(documento);
                if (string.IsNullOrWhiteSpace(docLimpio)) return matches;

                _logger.LogDebug("🔍 Buscando por documento: '{DocumentoOriginal}' → '{DocumentoLimpio}'",
                    documento, docLimpio);

                // Obtener todos los clientes y buscar localmente (por ahora)
                // TODO: Optimizar esto cuando Velneo soporte búsqueda por documento específica
                var todosLosClientes = await _velneoService.GetClientesAsync();

                foreach (var cliente in todosLosClientes)
                {
                    if (string.IsNullOrWhiteSpace(cliente.Cliruc)) continue;

                    var docClienteLimpio = LimpiarDocumento(cliente.Cliruc);

                    if (docLimpio.Equals(docClienteLimpio, StringComparison.OrdinalIgnoreCase))
                    {
                        var match = CrearClienteMatch(cliente, datosOriginales, 100, "Documento exacto");
                        match.Coincidencias.Add($"Documento: {docLimpio}");
                        matches.Add(match);

                        _logger.LogInformation("✅ Match exacto por documento: Cliente {ClienteId} - {ClienteNombre}",
                            cliente.Id, cliente.Clinom);
                        break; // Documento debe ser único
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando por documento: {Documento}", documento);
            }

            return matches;
        }

        private async Task<List<ClienteMatch>> BuscarPorNombre(string nombre, DatosClienteExtraidos datosOriginales)
        {
            var matches = new List<ClienteMatch>();

            try
            {
                var nombreLimpio = LimpiarNombre(nombre);
                if (string.IsNullOrWhiteSpace(nombreLimpio)) return matches;

                _logger.LogDebug("🔍 Buscando por nombre: '{NombreOriginal}' → '{NombreLimpio}'",
                    nombre, nombreLimpio);

                // Usar el método de búsqueda existente que ya maneja texto
                var clientesEncontrados = await _velneoService.SearchClientesAsync(nombreLimpio);

                foreach (var cliente in clientesEncontrados)
                {
                    var score = CalcularSimilitudNombre(nombreLimpio, cliente.Clinom ?? "");

                    if (score >= 50) // Solo coincidencias razonables
                    {
                        var criterio = score >= 90 ? "Nombre muy similar" :
                                      score >= 75 ? "Nombre similar" : "Nombre parcialmente similar";

                        var match = CrearClienteMatch(cliente, datosOriginales, score, criterio);
                        match.Coincidencias.Add($"Nombre: {score}% similar");

                        // Verificar otros campos para aumentar confianza
                        if (!string.IsNullOrWhiteSpace(datosOriginales.Email) &&
                            !string.IsNullOrWhiteSpace(cliente.Cliemail) &&
                            datosOriginales.Email.Equals(cliente.Cliemail, StringComparison.OrdinalIgnoreCase))
                        {
                            match.Score += 20; // Bonus por email coincidente
                            match.Coincidencias.Add("Email coincidente");
                        }

                        matches.Add(match);
                    }
                }

                matches = matches.OrderByDescending(m => m.Score).ToList();

                _logger.LogInformation("🔍 Búsqueda por nombre encontró {Count} coincidencias", matches.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando por nombre: {Nombre}", nombre);
            }

            return matches;
        }

        private async Task<List<ClienteMatch>> BuscarPorEmail(string email, DatosClienteExtraidos datosOriginales)
        {
            var matches = new List<ClienteMatch>();

            try
            {
                var emailLimpio = email.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(emailLimpio)) return matches;

                _logger.LogDebug("🔍 Buscando por email: {Email}", emailLimpio);

                // Usar búsqueda de texto que incluye email
                var clientesEncontrados = await _velneoService.SearchClientesAsync(emailLimpio);

                foreach (var cliente in clientesEncontrados)
                {
                    if (!string.IsNullOrWhiteSpace(cliente.Cliemail) &&
                        cliente.Cliemail.Equals(emailLimpio, StringComparison.OrdinalIgnoreCase))
                    {
                        var match = CrearClienteMatch(cliente, datosOriginales, 85, "Email exacto");
                        match.Coincidencias.Add($"Email: {emailLimpio}");
                        matches.Add(match);
                    }
                }

                _logger.LogInformation("🔍 Búsqueda por email encontró {Count} coincidencias", matches.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error buscando por email: {Email}", email);
            }

            return matches;
        }

        #endregion

        #region Métodos de Utilidad

        private ClienteMatch CrearClienteMatch(ClientDto cliente, DatosClienteExtraidos datosOriginales,
            int scoreInicial, string criterio)
        {
            var match = new ClienteMatch
            {
                Cliente = cliente,
                Score = scoreInicial,
                Criterio = criterio,
                Coincidencias = new List<string>(),
                Diferencias = new List<string>()
            };

            // Analizar diferencias en datos principales
            AnalizarDiferencias(match, datosOriginales);

            return match;
        }

        private void AnalizarDiferencias(ClienteMatch match, DatosClienteExtraidos datosOriginales)
        {
            var cliente = match.Cliente;

            // Comparar nombre
            if (!string.IsNullOrWhiteSpace(datosOriginales.Nombre) &&
                !string.IsNullOrWhiteSpace(cliente.Clinom))
            {
                var similitud = CalcularSimilitudNombre(datosOriginales.Nombre, cliente.Clinom);
                if (similitud < 80)
                {
                    match.Diferencias.Add($"Nombre: '{datosOriginales.Nombre}' vs '{cliente.Clinom}'");
                }
            }

            // Comparar documento
            if (!string.IsNullOrWhiteSpace(datosOriginales.Documento) &&
                !string.IsNullOrWhiteSpace(cliente.Cliruc))
            {
                var docOriginal = LimpiarDocumento(datosOriginales.Documento);
                var docCliente = LimpiarDocumento(cliente.Cliruc);

                if (!docOriginal.Equals(docCliente, StringComparison.OrdinalIgnoreCase))
                {
                    match.Diferencias.Add($"Documento: '{docOriginal}' vs '{docCliente}'");
                }
            }

            // Comparar email
            if (!string.IsNullOrWhiteSpace(datosOriginales.Email) &&
                !string.IsNullOrWhiteSpace(cliente.Cliemail))
            {
                if (!datosOriginales.Email.Equals(cliente.Cliemail, StringComparison.OrdinalIgnoreCase))
                {
                    match.Diferencias.Add($"Email: '{datosOriginales.Email}' vs '{cliente.Cliemail}'");
                }
            }
        }

        private ClienteMatchResult CrearResultado(List<ClienteMatch> matches, DatosClienteExtraidos datosOriginales)
        {
            var resultado = new ClienteMatchResult
            {
                Matches = matches,
                DatosOriginales = datosOriginales
            };

            if (!matches.Any())
            {
                resultado.TipoResultado = TipoResultadoCliente.SinCoincidencias;
                resultado.MensajeUsuario = "No se encontraron clientes coincidentes. Considere crear un cliente nuevo.";
                resultado.RequiereIntervencionManual = true;
            }
            else if (matches.Count == 1 && matches[0].Score >= 95)
            {
                resultado.TipoResultado = TipoResultadoCliente.MatchExacto;
                resultado.MensajeUsuario = $"Cliente encontrado con alta confianza: {matches[0].Cliente.Clinom}";
                resultado.RequiereIntervencionManual = false;
            }
            else if (matches.Count == 1 && matches[0].Score >= 85)
            {
                resultado.TipoResultado = TipoResultadoCliente.MatchMuyProbable;
                resultado.MensajeUsuario = $"Cliente muy probable: {matches[0].Cliente.Clinom}. Confirme antes de continuar.";
                resultado.RequiereIntervencionManual = true;
            }
            else if (matches.Count > 1 && matches.Any(m => m.Score >= 70))
            {
                resultado.TipoResultado = TipoResultadoCliente.MultiplesMatches;
                resultado.MensajeUsuario = $"Se encontraron {matches.Count} clientes similares. Seleccione el correcto.";
                resultado.RequiereIntervencionManual = true;
            }
            else
            {
                resultado.TipoResultado = TipoResultadoCliente.MatchParcial;
                resultado.MensajeUsuario = "Se encontraron coincidencias parciales. Revise cuidadosamente.";
                resultado.RequiereIntervencionManual = true;
            }

            _logger.LogInformation("🎯 Resultado búsqueda: {TipoResultado} - {Count} matches - Requiere intervención: {RequiereIntervención}",
                resultado.TipoResultado, matches.Count, resultado.RequiereIntervencionManual);

            return resultado;
        }

        private string LimpiarDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento)) return "";

            // Remover puntos, guiones, espacios
            var limpio = Regex.Replace(documento, @"[^\d]", "");

            // Remover ceros iniciales para CI uruguayas
            limpio = limpio.TrimStart('0');

            return limpio;
        }

        private string LimpiarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "";

            // Normalizar espacios y caracteres especiales
            var limpio = Regex.Replace(nombre.Trim().ToUpper(), @"\s+", " ");

            // Remover caracteres especiales comunes
            limpio = limpio.Replace(".", "").Replace(",", "").Replace("-", " ");

            return limpio;
        }

        private int CalcularSimilitudNombre(string nombre1, string nombre2)
        {
            if (string.IsNullOrWhiteSpace(nombre1) || string.IsNullOrWhiteSpace(nombre2))
                return 0;

            var n1 = LimpiarNombre(nombre1);
            var n2 = LimpiarNombre(nombre2);

            // Coincidencia exacta
            if (n1.Equals(n2, StringComparison.OrdinalIgnoreCase))
                return 100;

            // Calcular distancia de Levenshtein normalizada
            var distancia = CalcularDistanciaLevenshtein(n1, n2);
            var longitudMaxima = Math.Max(n1.Length, n2.Length);

            if (longitudMaxima == 0) return 0;

            var similitud = (int)((1.0 - (double)distancia / longitudMaxima) * 100);

            // Bonus si contiene palabras en común
            var palabras1 = n1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var palabras2 = n2.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var palabrasComunes = palabras1.Intersect(palabras2, StringComparer.OrdinalIgnoreCase).Count();

            if (palabrasComunes > 0)
            {
                var bonusPalabras = (palabrasComunes * 20) / Math.Max(palabras1.Length, palabras2.Length);
                similitud = Math.Min(100, similitud + bonusPalabras);
            }

            return Math.Max(0, similitud);
        }

        private int CalcularDistanciaLevenshtein(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1)) return s2?.Length ?? 0;
            if (string.IsNullOrEmpty(s2)) return s1.Length;

            var matriz = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                matriz[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                matriz[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    var costo = s1[i - 1] == s2[j - 1] ? 0 : 1;
                    matriz[i, j] = Math.Min(Math.Min(
                        matriz[i - 1, j] + 1,
                        matriz[i, j - 1] + 1),
                        matriz[i - 1, j - 1] + costo);
                }
            }

            return matriz[s1.Length, s2.Length];
        }

        #endregion
    }
}