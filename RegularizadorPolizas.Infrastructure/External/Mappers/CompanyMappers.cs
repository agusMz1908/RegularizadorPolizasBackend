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
                Nombre = velneoCompany.Nombre,
                Codigo = velneoCompany.Codigo,
                Descripcion = velneoCompany.Descripcion,
                Activo = velneoCompany.Activo,
                FechaCreacion = velneoCompany.FechaCreacion,
                FechaModificacion = velneoCompany.FechaModificacion,
                Direccion = velneoCompany.Direccion,
                Telefono = velneoCompany.Telefono,
                Email = velneoCompany.Email,
                Website = velneoCompany.Website,
                Ruc = velneoCompany.Ruc,
                ContactoPrincipal = velneoCompany.ContactoPrincipal,
                TelefonoContacto = velneoCompany.TelefonoContacto,
                EmailContacto = velneoCompany.EmailContacto
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
                Nombre = companyDto.Nombre,
                Codigo = companyDto.Codigo,
                Descripcion = companyDto.Descripcion,
                Activo = companyDto.Activo,
                FechaCreacion = companyDto.FechaCreacion,
                FechaModificacion = companyDto.FechaModificacion,
                Direccion = companyDto.Direccion,
                Telefono = companyDto.Telefono,
                Email = companyDto.Email,
                Website = companyDto.Website,
                Ruc = companyDto.Ruc,
                ContactoPrincipal = companyDto.ContactoPrincipal,
                TelefonoContacto = companyDto.TelefonoContacto,
                EmailContacto = companyDto.EmailContacto
            };
        }

        public static IEnumerable<VelneoCompany> ToVelneoCompanyDtos(this IEnumerable<CompanyDto> companyDtos)
        {
            return companyDtos.Select(c => c.ToVelneoCompanyDto());
        }

        // Mapper específico para lookups/selects
        public static CompanyLookupDto ToCompanyLookupDto(this VelneoCompany velneoCompany)
        {
            return new CompanyLookupDto
            {
                Id = velneoCompany.Id,
                Nombre = velneoCompany.Nombre,
                Codigo = velneoCompany.Codigo,
                Activo = velneoCompany.Activo
            };
        }

        public static IEnumerable<CompanyLookupDto> ToCompanyLookupDtos(this IEnumerable<VelneoCompany> velneoCompanies)
        {
            return velneoCompanies.Select(c => c.ToCompanyLookupDto());
        }

        // Mapper desde VelneoCompanyLookup (si Velneo tiene endpoints específicos para lookups)
        public static CompanyLookupDto ToCompanyLookupDto(this VelneoCompanyLookup velneoCompanyLookup)
        {
            return new CompanyLookupDto
            {
                Id = velneoCompanyLookup.Id,
                Nombre = velneoCompanyLookup.Nombre,
                Codigo = velneoCompanyLookup.Codigo,
                Activo = velneoCompanyLookup.Activo
            };
        }

        public static IEnumerable<CompanyLookupDto> ToCompanyLookupDtos(this IEnumerable<VelneoCompanyLookup> velneoCompanyLookups)
        {
            return velneoCompanyLookups.Select(c => c.ToCompanyLookupDto());
        }
    }
}