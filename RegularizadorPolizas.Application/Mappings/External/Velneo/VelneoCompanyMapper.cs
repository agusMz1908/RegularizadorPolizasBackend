using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.External.Velneo;

namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Mappers
{
    public static class VelneoCompanyMapper
    {
        public static CompanyDto ToCompanyDto(this VelneoCompanyDto velneoCompany)
        {
            return new CompanyDto
            {
                Id = velneoCompany.Id,
                Comnom = velneoCompany.Comnom,
                Comrazsoc = velneoCompany.Comrazsoc,
                Comruc = velneoCompany.Comruc,
                Comdom = velneoCompany.Comdom,
                Comtel = velneoCompany.Comtel,
                Comfax = velneoCompany.Comfax,
                Comalias = velneoCompany.Comalias,
                Broker = velneoCompany.Broker,
                Cod_srvcompanias = velneoCompany.Cod_srvcompanias,
                No_utiles = velneoCompany.No_utiles,
                Paq_dias = velneoCompany.Paq_dias,
                Comcntcli = velneoCompany.Comcntcli,
                Comcntcon = velneoCompany.Comcntcon,
                Comprepes = velneoCompany.Comprepes,
                Compredol = velneoCompany.Compredol,
                Comcomipe = velneoCompany.Comcomipe,
                Comcomido = velneoCompany.Comcomido,
                Comtotcomi = velneoCompany.Comtotcomi,
                Comtotpre = velneoCompany.Comtotpre,
                Comlog = velneoCompany.Comlog,
                Comsumodia = velneoCompany.Comsumodia,
                Activo = velneoCompany.Activo,
                TotalPolizas = 0,
            };
        }

        public static IEnumerable<CompanyDto> ToCompanyDtos(this IEnumerable<VelneoCompanyDto> velneoCompanies)
        {
            return velneoCompanies.Select(vc => vc.ToCompanyDto());
        }

        public static VelneoCompanyDto ToVelneoCompanyDto(this CompanyDto companyDto)
        {
            return new VelneoCompanyDto
            {
                Id = companyDto.Id,
                Comnom = companyDto.Comnom,
                Comrazsoc = companyDto.Comrazsoc,
                Comruc = companyDto.Comruc,
                Comdom = companyDto.Comdom,
                Comtel = companyDto.Comtel,
                Comfax = companyDto.Comfax,
                Comalias = companyDto.Comalias,
                Broker = companyDto.Broker,
                Cod_srvcompanias = companyDto.Cod_srvcompanias,
                No_utiles = companyDto.No_utiles,
                Paq_dias = companyDto.Paq_dias,
                Comcntcli = companyDto.Comcntcli,
                Comcntcon = companyDto.Comcntcon,
                Comprepes = companyDto.Comprepes,
                Compredol = companyDto.Compredol,
                Comcomipe = companyDto.Comcomipe,
                Comcomido = companyDto.Comcomido,
                Comtotcomi = companyDto.Comtotcomi,
                Comtotpre = companyDto.Comtotpre,
                Comlog = companyDto.Comlog,
                Comsumodia = companyDto.Comsumodia
            };
        }
    }
}