using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class SeccionMappers
    {
        public static SeccionDto ToSeccionDto(this VelneoSeccion velneoSeccion)
        {
            return new SeccionDto
            {
                Id = velneoSeccion.Id,
                Name = velneoSeccion.Name,
                Icono = velneoSeccion.Icono,
                Activo = velneoSeccion.Activo,
                FechaCreacion = velneoSeccion.FechaCreacion,
                FechaModificacion = velneoSeccion.FechaModificacion
            };
        }

        public static IEnumerable<SeccionDto> ToSeccionDtos(this IEnumerable<VelneoSeccion> velneoSecciones)
        {
            return velneoSecciones.Select(s => s.ToSeccionDto());
        }

        public static SeccionLookupDto ToSeccionLookupDto(this VelneoSeccionLookup velneoSeccionLookup)
        {
            return new SeccionLookupDto
            {
                Id = velneoSeccionLookup.Id,
                Name = velneoSeccionLookup.Name,
                Icono = velneoSeccionLookup.Icono,
                Activo = velneoSeccionLookup.Activo
            };
        }

        public static IEnumerable<SeccionLookupDto> ToSeccionLookupDtos(this IEnumerable<VelneoSeccionLookup> velneoSeccionesLookup)
        {
            return velneoSeccionesLookup.Select(s => s.ToSeccionLookupDto());
        }

        public static VelneoSeccion ToVelneoSeccionDto(this SeccionDto seccionDto)
        {
            return new VelneoSeccion
            {
                Id = seccionDto.Id,
                Name = seccionDto.Name,
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