using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Client, ClientDto>()
                .ReverseMap();

            CreateMap<Poliza, PolizaDto>()
                .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.Clinom, opt => opt.MapFrom(src => src.Client != null ? src.Client.Clinom : null))
                .ReverseMap()
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.PolizaPadre, opt => opt.Ignore())
                .ForMember(dest => dest.PolizasHijas, opt => opt.Ignore())
                .ForMember(dest => dest.ProcessDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.RenovacionesOrigen, opt => opt.Ignore())
                .ForMember(dest => dest.RenovacionesDestino, opt => opt.Ignore());

            CreateMap<ProcessDocument, ProcessDocumentDto>()
                .ReverseMap();

            CreateMap<Renovation, RenovationDto>()
                .ForMember(dest => dest.NombrePoliza, opt => opt.MapFrom(src =>
                    src.PolizaOriginal != null ? $"{src.PolizaOriginal.Conpol} - {src.PolizaOriginal.Conmaraut}" : null))
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.Nombre : null))
                .ReverseMap()
                .ForMember(dest => dest.PolizaOriginal, opt => opt.Ignore())
                .ForMember(dest => dest.PolizaNueva, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<User, UserDto>()
                .ReverseMap();

            CreateMap<Company, CompanyDto>()
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore()); 

            CreateMap<Company, CompanyLookupDto>();

            CreateMap<Broker, BrokerDto>()
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore());

            CreateMap<Broker, BrokerLookupDto>();

            CreateMap<Currency, CurrencyDto>()
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore());

            // Mapeo Currency <-> CurrencyLookupDto (simplificado para dropdowns)
            CreateMap<Currency, CurrencyLookupDto>();
        }
    }
}