using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
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

        public static SeccionDto ToSeccionDto(this VelneoSeccion velneoSeccion)
        {
            var iconoAsignado = !string.IsNullOrEmpty(velneoSeccion.Icono)
                ? velneoSeccion.Icono
                : GetIconoForSeccion(velneoSeccion.Seccion);

            return new SeccionDto
            {
                Id = velneoSeccion.Id,
                Seccion = velneoSeccion.Seccion,
                Icono = iconoAsignado,
                Activo = true, 
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow
            };
        }

        public static IEnumerable<SeccionDto> ToSeccionDtos(this IEnumerable<VelneoSeccion> velneoSecciones)
        {
            return velneoSecciones.Select(s => s.ToSeccionDto());
        }

        public static SeccionLookupDto ToSeccionLookupDto(this VelneoSeccionLookup velneoSeccionLookup)
        {
            var iconoAsignado = !string.IsNullOrEmpty(velneoSeccionLookup.Icono)
                ? velneoSeccionLookup.Icono
                : IconosPorSeccion.GetValueOrDefault(velneoSeccionLookup.Seccion.ToUpper(), "📋");

            return new SeccionLookupDto
            {
                Id = velneoSeccionLookup.Id,
                Name = velneoSeccionLookup.Seccion,
                Icono = iconoAsignado,
                Activo = velneoSeccionLookup.Activo
            };
        }

        public static IEnumerable<SeccionLookupDto> ToSeccionLookupDtos(this IEnumerable<VelneoSeccionLookup> velneoSeccionesLookup)
        {
            return velneoSeccionesLookup.Select(s => s.ToSeccionLookupDto());
        }

        // Mapeo inverso (no se usa para Velneo API pero mantenemos por compatibilidad)
        public static VelneoSeccion ToVelneoSeccionDto(this SeccionDto seccionDto)
        {
            return new VelneoSeccion
            {
                Id = seccionDto.Id,
                Seccion = seccionDto.Seccion, 
                Icono = seccionDto.Icono,
                Activo = seccionDto.Activo,
                FechaCreacion = seccionDto.FechaCreacion,
                FechaModificacion = seccionDto.FechaModificacion
            };
        }

        public static IEnumerable<VelneoSeccion> ToVelneoSeccionDtos(this IEnumerable<SeccionDto> seccionDtos)
        {
            return seccionDtos.Select(s => s.ToVelneoSeccionDto());
        }
    }
}
