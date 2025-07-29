using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Extensions
{
    public static partial class VelneoMappingExtensions
    {
        #region Extensiones para Coberturas
        public static CoberturaDto ToCoberturaDto(this VelneoCobertura velneo)
        {
            return new CoberturaDto
            {
                Id = velneo.Id,
                Descripcion = velneo.Descripcion,
                Codigo = velneo.Codigo,
                Activo = velneo.Activo,
                Observaciones = velneo.Observaciones
            };
        }

        public static IEnumerable<CoberturaDto> ToCoberturasDtos(this IEnumerable<VelneoCobertura> velneos)
        {
            return velneos.Select(v => v.ToCoberturaDto());
        }

        #endregion

        #region Extensiones para Departamentos
        public static DepartamentoDto ToDepartamentoDto(this VelneoDepartamento velneo)
        {
            return new DepartamentoDto
            {
                Id = velneo.Id,
                Nombre = velneo.Nombre,
                BonificacionInterior = velneo.BonificacionInterior,
                CodigoSC = velneo.CodigoSC,
                Activo = velneo.Activo
            };
        }

        public static IEnumerable<DepartamentoDto> ToDepartamentosDtos(this IEnumerable<VelneoDepartamento> velneos)
        {
            return velneos.Select(v => v.ToDepartamentoDto());
        }

        #endregion
    }
}