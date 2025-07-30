using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class CalidadMappers
    {
        public static CalidadDto ToCalidadDto(this VelneoCalidad velneoCalidad)
        {
            return new CalidadDto
            {
                Id = velneoCalidad.Id,
                Caldsc = velneoCalidad.Caldsc,
                Calcod = velneoCalidad.Calcod
            };
        }

        public static IEnumerable<CalidadDto> ToCalidadDtos(this IEnumerable<VelneoCalidad> velneoCalidades)
        {
            return velneoCalidades.Select(c => c.ToCalidadDto());
        }
    }
}