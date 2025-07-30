using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class CategoriaMappers
    {
        public static CategoriaDto ToCategoriaDto(this VelneoCategoria velneoCategoria)
        {
            return new CategoriaDto
            {
                Id = velneoCategoria.Id,
                Catdsc = velneoCategoria.Catdsc,
                Catcod = velneoCategoria.Catcod
            };
        }

        public static IEnumerable<CategoriaDto> ToCategoriaDtos(this IEnumerable<VelneoCategoria> velneoCategorias)
        {
            return velneoCategorias.Select(c => c.ToCategoriaDto());
        }
    }
}