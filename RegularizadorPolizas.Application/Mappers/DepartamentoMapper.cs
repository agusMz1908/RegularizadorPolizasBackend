using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class DepartamentoMappers
    {
        public static DepartamentoDto ToDepartamentoDto(this VelneoDepartamento velneoDepartamento)
        {
            return new DepartamentoDto
            {
                Id = velneoDepartamento.Id,
                Nombre = velneoDepartamento.Nombre,
                BonificacionInterior = velneoDepartamento.BonificacionInterior,
                CodigoSC = velneoDepartamento.CodigoSC,
                Activo = velneoDepartamento.Activo
            };
        }

        public static IEnumerable<DepartamentoDto> ToDepartamentosDtos(this IEnumerable<VelneoDepartamento> velneoDepartamentos)
        {
            return velneoDepartamentos.Select(v => v.ToDepartamentoDto());
        }
    }
}