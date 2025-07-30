using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class SeccionMappers
    {
        private static readonly Dictionary<string, string> IconosPorSeccion = new()
        {
            { "AUTOMOVILES", "🚗" },
            { "VIDA", "👤" },
            { "INCENDIO", "🔥" },
            { "ACCIDENTES", "⚠️" },
            { "TRANSPORTE", "🚛" },
            { "RURALES", "🌾" },
            { "CRISTALES", "🪟" },
            { "CAUCIONES", "📋" },
            { "FIANZAS", "🤝" },
            { "DECLARACIONES", "📄" },
            { "EMBARCACIONES", "⛵" },
            { "RC", "⚖️" },
            { "FLOTANTE", "📦" },
            { "VIAJERO", "✈️" },
            { "AUTO + CASA", "🏠🚗" },
            { "BICICLETAS", "🚲" },
            { "SALUD", "🏥" },
            { "ALQUILER", "🏘️" },
            { "FLOTA", "🚛" }
        };

        public static string GetIconoForSeccion(string seccion)
        {
            return IconosPorSeccion.GetValueOrDefault(seccion?.ToUpper() ?? "", "📋");
        }

        /// <summary>
        /// ✅ CORREGIDO: Usa las propiedades reales de VelneoSeccion (Secdsc, Seccod)
        /// </summary>
        public static SeccionDto ToSeccionDto(this VelneoSeccion velneoSeccion)
        {
            // ✅ USAR SECDSC como nombre principal de la sección
            var nombreSeccion = velneoSeccion.Secdsc ?? velneoSeccion.Seccod ?? $"Sección {velneoSeccion.Id}";

            return new SeccionDto
            {
                Id = velneoSeccion.Id,
                Seccion = nombreSeccion,  // ✅ Usar Secdsc como nombre
                Icono = GetIconoForSeccion(nombreSeccion),
                Activo = velneoSeccion.Activo,
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow
            };
        }

        public static IEnumerable<SeccionDto> ToSeccionDtos(this IEnumerable<VelneoSeccion> velneoSecciones)
        {
            return velneoSecciones.Select(s => s.ToSeccionDto());
        }

        /// <summary>
        /// ✅ SeccionDto a SeccionLookupDto
        /// </summary>
        public static SeccionLookupDto ToSeccionLookupDto(this SeccionDto seccionDto)
        {
            return new SeccionLookupDto
            {
                Id = seccionDto.Id,
                Name = seccionDto.Seccion,
                Icono = seccionDto.Icono,
                Activo = seccionDto.Activo
            };
        }

        public static IEnumerable<SeccionLookupDto> ToSeccionLookupDtos(this IEnumerable<SeccionDto> seccionDtos)
        {
            return seccionDtos.Select(s => s.ToSeccionLookupDto());
        }

        /// <summary>
        /// ✅ DIRECTO: VelneoSeccion a SeccionLookupDto (para código existente)
        /// </summary>
        public static SeccionLookupDto ToSeccionLookupDtoFromVelneo(this VelneoSeccion velneoSeccion)
        {
            var nombreSeccion = velneoSeccion.Secdsc ?? velneoSeccion.Seccod ?? $"Sección {velneoSeccion.Id}";

            return new SeccionLookupDto
            {
                Id = velneoSeccion.Id,
                Name = nombreSeccion,
                Icono = GetIconoForSeccion(nombreSeccion),
                Activo = velneoSeccion.Activo
            };
        }
    }
}