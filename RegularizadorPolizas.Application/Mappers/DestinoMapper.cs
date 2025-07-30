using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class DestinoMappers
    {
        public static DestinoDto ToDestinoDto(this VelneoDestino velneoDestino)
        {
            return new DestinoDto
            {
                Id = velneoDestino.Id,
                Desnom = velneoDestino.Desnom,
                Descod = velneoDestino.Descod
            };
        }

        public static IEnumerable<DestinoDto> ToDestinoDtos(this IEnumerable<VelneoDestino> velneoDestinos)
        {
            return velneoDestinos.Select(d => d.ToDestinoDto());
        }
    }
}