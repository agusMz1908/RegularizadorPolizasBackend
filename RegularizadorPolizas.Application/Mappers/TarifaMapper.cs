using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class TarifaMappers
    {
        public static TarifaDto ToTarifaDto(this VelneoTarifa velneoTarifa)
        {
            return new TarifaDto
            {
                Id = velneoTarifa.Id,
                CompaniaId = velneoTarifa.Companias,
                Nombre = CleanAndValidateName(velneoTarifa.TarNom),
                Codigo = CleanOptionalField(velneoTarifa.TarCod),
                Descripcion = CleanDescription(velneoTarifa.TarDsc),
                Activa = true // Todas las tarifas de Velneo se consideran activas
            };
        }

        public static IEnumerable<TarifaDto> ToTarifaDtos(this IEnumerable<VelneoTarifa> velneoTarifas)
        {
            return velneoTarifas.Select(t => t.ToTarifaDto());
        }

        public static TarifaLookupDto ToTarifaLookupDto(this VelneoTarifa velneoTarifa)
        {
            return new TarifaLookupDto
            {
                Id = velneoTarifa.Id,
                CompaniaId = velneoTarifa.Companias,
                Nombre = CleanAndValidateName(velneoTarifa.TarNom),
                Codigo = CleanOptionalField(velneoTarifa.TarCod)
            };
        }

        public static IEnumerable<TarifaLookupDto> ToTarifaLookupDtos(this IEnumerable<VelneoTarifa> velneoTarifas)
        {
            return velneoTarifas.Select(t => t.ToTarifaLookupDto());
        }

        public static IEnumerable<TarifaStatsDto> ToTarifaStats(this IEnumerable<VelneoTarifa> velneoTarifas)
        {
            return velneoTarifas
                .GroupBy(t => t.Companias)
                .Select(group => new TarifaStatsDto
                {
                    CompaniaId = group.Key,
                    TotalTarifas = group.Count(),
                    TarifasPopulares = group
                        .Take(5) // Top 5 tarifas por compañía
                        .Select(t => t.ToTarifaLookupDto())
                        .ToList()
                })
                .OrderBy(stats => stats.CompaniaId);
        }

        #region Métodos privados de limpieza y validación

        private static string CleanAndValidateName(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "Sin nombre";

            return nombre.Trim();
        }
        private static string? CleanOptionalField(string? campo)
        {
            if (string.IsNullOrWhiteSpace(campo))
                return null;

            var cleaned = campo.Trim();
            return string.IsNullOrEmpty(cleaned) ? null : cleaned;
        }
        private static string? CleanDescription(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return null;

            var cleaned = descripcion.Trim();
            return string.IsNullOrEmpty(cleaned) ? null : cleaned;
        }

        #endregion
    }
}