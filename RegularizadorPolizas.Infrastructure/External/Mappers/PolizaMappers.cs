using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class PolizaMappers
    {
        public static PolizaDto ToPolizaDto(this VelneoPoliza velneoPoliza)
        {
            return new PolizaDto
            {
                Id = velneoPoliza.Id,
                Conpol = velneoPoliza.Numero,
                Clinro = velneoPoliza.ClienteId,
                Comcod = velneoPoliza.CompaniaId,
                Seccod = velneoPoliza.SeccionId,
                Confchdes = velneoPoliza.VigenciaDesde,
                Confchhas = velneoPoliza.VigenciaHasta,
                Conpremio = velneoPoliza.Premio,
                Contot = velneoPoliza.Suma,
                Concomcorr = velneoPoliza.Comision,
                Moncod = GetMonedaCodigo(velneoPoliza.Moneda),
                Conend = velneoPoliza.Endoso?.ToString(),
                Convig = velneoPoliza.Activa ? "1" : "0",
                Conimp = velneoPoliza.Impuestos,
                Observaciones = velneoPoliza.Observaciones,
                Corrnom = velneoPoliza.BrokerId,

                // Campos adicionales de la aplicación
                Activo = velneoPoliza.Activa,
                FechaCreacion = velneoPoliza.FechaCreacion,
                FechaModificacion = velneoPoliza.FechaModificacion ?? DateTime.Now,
                Procesado = true,

                // Información anidada si está presente
                Clinom = velneoPoliza.Cliente?.Nombre,

                // Campos por defecto para PolizaDto
                Rieres = "0",
                Conges = "N", // Nueva
                Congesti = "A", // Alta
                Consta = "ACT", // Activa
                Condom = string.Empty,
                Conmaraut = string.Empty,
                Conanioaut = null,
                Concodrev = null,
                Conmataut = string.Empty,
                Conficto = null,
                Conmotor = string.Empty,
                Conpadaut = string.Empty,
                Conchasis = string.Empty,
                Conclaaut = null,
                Condedaut = null,
                Conresciv = null,
                Conbonnsin = null,
                Conbonant = null,
                Concaraut = null,
                Concesnom = string.Empty,
                Concestel = string.Empty,
                Concapaut = null,
                Concuo = null,
                Catdsc = null,
                Desdsc = null,
                Caldsc = null,
                Flocod = null,
                Concar = string.Empty,
                Connroser = null,
                Congesfi = null,
                Congeses = string.Empty,
                Concan = null,
                Congrucon = string.Empty,
                Contipoemp = string.Empty,
                Conmatpar = string.Empty,
                Conmatte = string.Empty,
                Concapla = null,
                Conflota = null,
                Condednum = null,
                Contra = string.Empty,
                Conconf = string.Empty,
                Conpadre = null
            };
        }

        public static IEnumerable<PolizaDto> ToPolizaDtos(this IEnumerable<VelneoPoliza> velneoPolizas)
        {
            return velneoPolizas.Select(p => p.ToPolizaDto());
        }

        public static VelneoPoliza ToVelneoPolizaDto(this PolizaDto polizaDto)
        {
            return new VelneoPoliza
            {
                Id = polizaDto.Id,
                Numero = polizaDto.Conpol ?? string.Empty,
                ClienteId = polizaDto.Clinro ?? 0,
                CompaniaId = polizaDto.Comcod ?? 0,
                SeccionId = polizaDto.Seccod ?? 0,
                Estado = polizaDto.Consta ?? "ACT",
                VigenciaDesde = polizaDto.Confchdes ?? DateTime.Now,
                VigenciaHasta = polizaDto.Confchhas ?? DateTime.Now.AddYears(1),
                Suma = polizaDto.Contot ?? 0,
                Premio = polizaDto.Conpremio ?? 0,
                Comision = polizaDto.Concomcorr ?? 0,
                Moneda = GetMonedaNombre(polizaDto.Moncod),
                Observaciones = polizaDto.Observaciones,
                FechaCreacion = polizaDto.FechaCreacion,
                FechaModificacion = polizaDto.FechaModificacion,
                Endoso = !string.IsNullOrEmpty(polizaDto.Conend) && int.TryParse(polizaDto.Conend, out int endoso) ? endoso : null,
                Impuestos = polizaDto.Conimp,
                Activa = polizaDto.Convig == "1" || polizaDto.Activo,
                BrokerId = polizaDto.Corrnom,

                // Campos adicionales que VelneoPoliza puede requerir
                UsuarioCreacion = "System",
                UsuarioModificacion = "System"
            };
        }

        public static IEnumerable<VelneoPoliza> ToVelneoPolizaDtos(this IEnumerable<PolizaDto> polizaDtos)
        {
            return polizaDtos.Select(p => p.ToVelneoPolizaDto());
        }

        // Métodos auxiliares para conversión de monedas
        private static int? GetMonedaCodigo(string monedaNombre)
        {
            return monedaNombre?.ToUpper() switch
            {
                "UYU" or "PESO" or "PESOS" => 1,
                "USD" or "DOLAR" or "DOLARES" => 2,
                "EUR" or "EURO" or "EUROS" => 3,
                _ => 1 // Default a peso uruguayo
            };
        }

        private static string GetMonedaNombre(int? monedaCodigo)
        {
            return monedaCodigo switch
            {
                1 => "UYU",
                2 => "USD",
                3 => "EUR",
                _ => "UYU" // Default a peso uruguayo
            };
        }
    }
}