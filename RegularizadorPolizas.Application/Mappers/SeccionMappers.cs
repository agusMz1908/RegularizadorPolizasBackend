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
        /// ✅ CORREGIDO: Usa los campos reales de VelneoSeccion
        /// </summary>
        public static SeccionDto ToSeccionDto(this VelneoSeccion velneoSeccion)
        {
            // ✅ USAR "Seccion" (el campo real) como nombre principal
            var nombreSeccion = velneoSeccion.Seccion ?? velneoSeccion.Seccod ?? $"Sección {velneoSeccion.Id}";

            // ✅ Usar el icono de Velneo si viene, sino usar el mapeo local
            var iconoSeccion = !string.IsNullOrEmpty(velneoSeccion.Icono)
                             ? velneoSeccion.Icono
                             : GetIconoForSeccion(nombreSeccion);

            return new SeccionDto
            {
                Id = velneoSeccion.Id,
                Seccion = nombreSeccion,  // ✅ Usar el campo "seccion" real de Velneo
                Icono = iconoSeccion,     // ✅ Priorizar icono de Velneo
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
        /// ✅ DIRECTO: VelneoSeccion a SeccionLookupDto (optimizado)
        /// </summary>
        public static SeccionLookupDto ToSeccionLookupDtoFromVelneo(this VelneoSeccion velneoSeccion)
        {
            var nombreSeccion = velneoSeccion.Seccion ?? velneoSeccion.Seccod ?? $"Sección {velneoSeccion.Id}";

            var iconoSeccion = !string.IsNullOrEmpty(velneoSeccion.Icono)
                             ? velneoSeccion.Icono
                             : GetIconoForSeccion(nombreSeccion);

            return new SeccionLookupDto
            {
                Id = velneoSeccion.Id,
                Name = nombreSeccion,
                Icono = iconoSeccion,
                Activo = velneoSeccion.Activo
            };
        }

        public static IEnumerable<SeccionLookupDto> ToSeccionLookupDtosFromVelneo(this IEnumerable<VelneoSeccion> velneoSecciones)
        {
            return velneoSecciones.Select(s => s.ToSeccionLookupDtoFromVelneo());
        }
    }
}