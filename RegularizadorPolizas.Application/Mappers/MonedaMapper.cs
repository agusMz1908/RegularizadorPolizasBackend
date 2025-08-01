﻿using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class MonedaMappers
    {
        public static MonedaDto ToMonedaDto(this VelneoMoneda velneoMoneda)
        {
            var nombreMoneda = GetNombreMoneda(velneoMoneda.Moneda);
            var simbolo = GetSimboloMoneda(velneoMoneda.Moneda);

            return new MonedaDto
            {
                Id = velneoMoneda.Id,
                Codigo = velneoMoneda.Moneda?.Trim() ?? string.Empty,
                Nombre = nombreMoneda,
                Simbolo = simbolo,
                Activa = true, // Asumimos que si está en la respuesta, está activa
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow
            };
        }
        public static IEnumerable<MonedaDto> ToMonedaDtos(this IEnumerable<VelneoMoneda> velneoMonedas)
        {
            return velneoMonedas.Select(vm => vm.ToMonedaDto());
        }

        private static string GetNombreMoneda(string? codigo)
        {
            return codigo?.ToUpperInvariant() switch
            {
                "PES" => "Peso Uruguayo",
                "DOL" => "Dólar Americano",
                "RS" => "Real Brasileño",
                "EU" => "Euro",
                "UF" => "Unidad de Fomento",
                _ => codigo ?? "Moneda Desconocida"
            };
        }

        private static string GetSimboloMoneda(string? codigo)
        {
            return codigo?.ToUpperInvariant() switch
            {
                "PES" => "$U",
                "DOL" => "$",
                "RS" => "R$",
                "EU" => "€",
                "UF" => "UF",
                _ => "¤"
            };
        }
    }
}