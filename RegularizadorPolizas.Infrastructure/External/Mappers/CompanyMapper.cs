using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Infrastructure.External.VelneoAPI.Models;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class CompanyMappers
    {
        public static CompanyDto ToCompanyDto(this VelneoCompany velneoCompany)
        {
            return new CompanyDto
            {
                Id = velneoCompany.Id,
                Comnom = velneoCompany.Nombre, // VelneoCompany.Nombre -> CompanyDto.Comnom
                Comrazsoc = velneoCompany.Descripcion ?? string.Empty,
                Comruc = velneoCompany.Ruc ?? string.Empty,
                Comdom = velneoCompany.Direccion ?? string.Empty,
                Comtel = velneoCompany.Telefono ?? string.Empty,
                Comfax = string.Empty,
                Comalias = velneoCompany.Codigo ?? string.Empty,
                Cod_srvcompanias = velneoCompany.Codigo ?? string.Empty,
                Activo = velneoCompany.Activo,
                Broker = false,
                No_utiles = 0,
                Paq_dias = 0,
                Comcntcli = 0,
                Comcntcon = 0,
                Comprepes = 0,
                Compredol = 0,
                Comcomipe = 0,
                Comcomido = 0,
                Comtotcomi = 0,
                Comtotpre = 0,
                Comlog = string.Empty,
                Comsumodia = string.Empty,
                TotalPolizas = 0
            };
        }

        public static IEnumerable<CompanyDto> ToCompanyDtos(this IEnumerable<VelneoCompany> velneoCompanies)
        {
            return velneoCompanies.Select(c => c.ToCompanyDto());
        }

        public static VelneoCompany ToVelneoCompanyDto(this CompanyDto companyDto)
        {
            return new VelneoCompany
            {
                Id = companyDto.Id,
                Nombre = companyDto.Comnom,
                Codigo = companyDto.Cod_srvcompanias,
                Descripcion = companyDto.Comrazsoc,
                Activo = companyDto.Activo,
                Direccion = companyDto.Comdom,
                Telefono = companyDto.Comtel,
                Email = string.Empty,
                Website = string.Empty,
                Ruc = companyDto.Comruc,
                ContactoPrincipal = string.Empty,
                TelefonoContacto = string.Empty,
                EmailContacto = string.Empty,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };
        }

        public static CompanyLookupDto ToCompanyLookupDto(this VelneoCompany velneoCompany)
        {
            return new CompanyLookupDto
            {
                Id = velneoCompany.Id,
                Comnom = velneoCompany.Nombre,
                Comalias = velneoCompany.Codigo ?? string.Empty,
                Cod_srvcompanias = velneoCompany.Codigo ?? string.Empty
            };
        }

        public static IEnumerable<CompanyLookupDto> ToCompanyLookupDtos(this IEnumerable<VelneoCompany> velneoCompanies)
        {
            return velneoCompanies.Select(c => c.ToCompanyLookupDto());
        }

        public static CompanyLookupDto ToCompanyLookupDto(this VelneoCompanyLookup velneoCompanyLookup)
        {
            return new CompanyLookupDto
            {
                Id = velneoCompanyLookup.Id,
                Comnom = velneoCompanyLookup.Nombre,
                Comalias = velneoCompanyLookup.Codigo ?? string.Empty,
                Cod_srvcompanias = velneoCompanyLookup.Codigo ?? string.Empty
            };
        }

        public static IEnumerable<CompanyLookupDto> ToCompanyLookupDtos(this IEnumerable<VelneoCompanyLookup> velneoCompanyLookups)
        {
            return velneoCompanyLookups.Select(c => c.ToCompanyLookupDto());
        }
    }
}