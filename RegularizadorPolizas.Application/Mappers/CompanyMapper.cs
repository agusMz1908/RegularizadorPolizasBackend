using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Models;

namespace RegularizadorPolizas.Application.Mappers
{
    public static class CompanyMappers
    {
        public static CompanyDto ToCompanyDto(this VelneoCompany velneoCompany)
        {
            return new CompanyDto
            {
                Id = velneoCompany.Id,

                Comnom = velneoCompany.Comnom ?? string.Empty,
                Comrazsoc = velneoCompany.Comrazsoc ?? string.Empty,
                Comruc = velneoCompany.Comruc ?? string.Empty,
                Comdom = velneoCompany.Comdom ?? string.Empty,
                Comtel = velneoCompany.Comtel ?? string.Empty,
                Comfax = velneoCompany.Comfax ?? string.Empty,
                Comalias = velneoCompany.Comalias ?? string.Empty,
                Cod_srvcompanias = velneoCompany.Cod_srvcompanias ?? string.Empty,

                Comcntcli = velneoCompany.Comcntcli,
                Comcntcon = velneoCompany.Comcntcon,
                Comprepes = velneoCompany.Comprepes,
                Compredol = velneoCompany.Compredol,
                Comcomipe = velneoCompany.Comcomipe,
                Comcomido = velneoCompany.Comcomido,
                Comtotcomi = velneoCompany.Comtotcomi,
                Comtotpre = velneoCompany.Comtotpre,
                Paq_dias = velneoCompany.Paq_dias,

                Comlog = velneoCompany.Comlog ?? string.Empty,
                Comsumodia = velneoCompany.Comsumodia ?? string.Empty,

                Broker = velneoCompany.Broker,
                Activo = true,

                No_utiles = ParseNoUtiles(velneoCompany.No_utiles),

                TotalPolizas = 0
            };
        }

        public static IEnumerable<CompanyDto> ToCompanyDtos(this IEnumerable<VelneoCompany> velneoCompanies)
        {
            return velneoCompanies.Select(c => c.ToCompanyDto());
        }

        public static CompanyLookupDto ToCompanyLookupDto(this VelneoCompany velneoCompany)
        {
            return new CompanyLookupDto
            {
                Id = velneoCompany.Id,
                Comnom = velneoCompany.Comnom ?? string.Empty,           
                Comalias = velneoCompany.Comalias ?? string.Empty,       
                Cod_srvcompanias = velneoCompany.Cod_srvcompanias ?? string.Empty 
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

        private static int ParseNoUtiles(string? noUtilesString)
        {
            if (string.IsNullOrEmpty(noUtilesString))
                return 0;

            if (int.TryParse(noUtilesString, out var numero))
                return numero;

            return 0;
        }
    }
}