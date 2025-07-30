using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class CoberturaMappers
    {
        public static CoberturaDto ToCoberturaDto(this VelneoCobertura velneoCobertura)
        {
            return new CoberturaDto
            {
                Id = velneoCobertura.Id,
                Descripcion = velneoCobertura.Descripcion,
                Codigo = velneoCobertura.Codigo ?? string.Empty,
                Activo = velneoCobertura.Activo,
                Observaciones = velneoCobertura.Observaciones
            };
        }

        public static IEnumerable<CoberturaDto> ToCoberturasDtos(this IEnumerable<VelneoCobertura> velneoCoberturas)
        {
            return velneoCoberturas.Select(v => v.ToCoberturaDto());
        }
    }
}