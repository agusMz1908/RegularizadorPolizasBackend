using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo Cliente <-> ClienteDto
            CreateMap<Client, ClientDto>()
                .ReverseMap();

            // Mapeo Poliza <-> PolizaDto
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

            // Mapeo DocumentoProcesado <-> DocumentoProcesadoDto
            CreateMap<ProcessDocument, ProcessDocumentDto>()
                .ReverseMap();

            // Mapeo Renovacion <-> RenovacionDto
            CreateMap<Renovation, RenovationDto>()
                .ForMember(dest => dest.NombrePoliza, opt => opt.MapFrom(src =>
                    src.PolizaOriginal != null ? $"{src.PolizaOriginal.Conpol} - {src.PolizaOriginal.Conmaraut}" : null))
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.Nombre : null))
                .ReverseMap()
                .ForMember(dest => dest.PolizaOriginal, opt => opt.Ignore())
                .ForMember(dest => dest.PolizaNueva, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Mapeo Usuario <-> UsuarioDto
            CreateMap<User, UserDto>()
                .ReverseMap();
        }
    }
}