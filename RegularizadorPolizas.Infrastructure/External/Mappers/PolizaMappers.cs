﻿using RegularizadorPolizas.Application.DTOs;
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
                Comcod = ParseIntFromObject(velneoPoliza.Comcod),
                Seccod = ParseIntFromObject(velneoPoliza.Seccod),
                Clinro = ParseIntFromObject(velneoPoliza.Clinro),
                Condom = velneoPoliza.Condom ?? "",

                Conmaraut = velneoPoliza.Conmaraut ?? "",
                Conanioaut = ParseIntFromObject(velneoPoliza.Conanioaut),
                Concodrev = ParseIntFromObject(velneoPoliza.Concodrev),
                Conmataut = velneoPoliza.Conmataut ?? "",
                Conficto = ParseIntFromObject(velneoPoliza.Conficto),
                Conmotor = velneoPoliza.Conmotor ?? "",
                Conpadaut = velneoPoliza.Conpadaut ?? "",
                Conchasis = velneoPoliza.Conchasis ?? "",

                Conclaaut = ParseIntFromObject(velneoPoliza.Conclaaut),
                Condedaut = ParseIntFromObject(velneoPoliza.Condedaut),
                Conresciv = ParseIntFromObject(velneoPoliza.Conresciv),
                Conbonnsin = ParseIntFromObject(velneoPoliza.Conbonnsin),
                Conbonant = ParseIntFromObject(velneoPoliza.Conbonant),
                Concaraut = ParseIntFromObject(velneoPoliza.Concaraut),

                Concesnom = velneoPoliza.Concesnom ?? "",
                Concestel = velneoPoliza.Concestel ?? "",
                Concapaut = ParseIntFromObject(velneoPoliza.Concapaut),

                Conpremio = ParseIntFromObject(velneoPoliza.Conpremio),
                Contot = ParseIntFromObject(velneoPoliza.Contot),
                Moncod = ParseIntFromObject(velneoPoliza.Moncod),
                Concuo = ParseIntFromObject(velneoPoliza.Concuo),
                Concomcorr = ParseIntFromObject(velneoPoliza.Concomcorr),

                Catdsc = velneoPoliza.Catdsc,
                Desdsc = velneoPoliza.Desdsc,
                Caldsc = velneoPoliza.Caldsc,
                Flocod = velneoPoliza.Flocod,

                Concar = velneoPoliza.Concar ?? "",
                Conpol = velneoPoliza.Conpol ?? "",
                Conend = velneoPoliza.Conend ?? "",
                Confchdes = ParseVelneoDate(velneoPoliza.Confchdes),
                Confchhas = ParseVelneoDate(velneoPoliza.Confchhas),
                Conimp = velneoPoliza.Conimp,
                Connroser = velneoPoliza.Connroser,
                Rieres = velneoPoliza.Rieres ?? "",

                Conges = velneoPoliza.Conges ?? "",
                Congesti = velneoPoliza.Congesti ?? "",
                Congesfi = ParseVelneoDate(velneoPoliza.Congesfi),
                Congeses = velneoPoliza.Congeses ?? "",
                Convig = velneoPoliza.Convig ?? "",
                Concan = velneoPoliza.Concan,
                Congrucon = velneoPoliza.Congrucon ?? "",

                Contipoemp = velneoPoliza.Contipoemp ?? "",
                Conmatpar = velneoPoliza.Conmatpar ?? "",
                Conmatte = velneoPoliza.Conmatte ?? "",
                Concapla = velneoPoliza.Concapla,
                Conflota = velneoPoliza.Conflota,
                Condednum = ParseIntFromObject(velneoPoliza.Condednum),
                Consta = velneoPoliza.Consta ?? "",
                Contra = velneoPoliza.Contra ?? "",
                Conconf = velneoPoliza.Conconf ?? "",
                Conpadre = velneoPoliza.Conpadre,
                Confchcan = ParseVelneoDate(velneoPoliza.Confchcan),
                Concaucan = velneoPoliza.Concaucan ?? "",
                Conobjtot = velneoPoliza.Conobjtot,

                Clinom = velneoPoliza.Clinom ?? "",
                Com_alias = velneoPoliza.ComAlias ?? "",
                Ramo = velneoPoliza.Ramo ?? "",
                Observaciones = velneoPoliza.Observaciones ?? "",

                Procesado = true,
                Activo = velneoPoliza.Consta != "3", 
                FechaCreacion = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow
            };
        }

        public static IEnumerable<PolizaDto> ToPolizaDtos(this IEnumerable<VelneoPoliza> velneoPolizas)
        {
            return velneoPolizas.Select(p => p.ToPolizaDto());
        }

        private static string MapPolizaEstado(string? consta)
        {
            if (string.IsNullOrEmpty(consta))
                return "Pendiente";

            return consta.ToLower() switch
            {
                "1" or "vigente" or "activo" => "Vigente",
                "2" or "vencida" or "vencido" => "Vencida",
                "3" or "cancelada" or "cancelado" => "Cancelada",
                "0" => "Inactiva", // Según tu JSON, consta="0"
                _ => "Pendiente"
            };
        }

        private static DateTime? ParseVelneoDate(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString) || dateString == "Invalid Date")
                return null;

            if (DateTime.TryParse(dateString, out var date))
                return date;

            return null;
        }

        private static int? ParseIntFromObject(object? value)
        {
            if (value == null) return null;

            if (value is int intValue) return intValue;

            if (int.TryParse(value.ToString(), out var parsedInt))
                return parsedInt;

            return null;
        }

        private static decimal? ParseDecimalFromObject(object? value)
        {
            if (value == null) return null;

            if (value is decimal decimalValue) return decimalValue;
            if (value is int intValue) return intValue;
            if (value is double doubleValue) return (decimal)doubleValue;
            if (value is float floatValue) return (decimal)floatValue;

            if (decimal.TryParse(value.ToString(), out var parsedDecimal))
                return parsedDecimal;

            return null;
        }
    }
}