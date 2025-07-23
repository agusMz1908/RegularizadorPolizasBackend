using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class CombustibleMappers
    {
        public static CombustibleDto ToCombustibleDto(this VelneoCombustible velneoCombustible)
        {
            return new CombustibleDto
            {
                Id = velneoCombustible.Id,
                Name = velneoCombustible.Name
            };
        }

        public static IEnumerable<CombustibleDto> ToCombustibleDtos(this IEnumerable<VelneoCombustible> velneoCombustibles)
        {
            return velneoCombustibles.Select(c => c.ToCombustibleDto());
        }
    }
}