using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Mappings
{
    public class ApiKeyMappingProfile : Profile
    {
        public ApiKeyMappingProfile()
        {
            CreateMap<ApiKey, ApiKeyDto>();

            CreateMap<ApiKeyCreateDto, ApiKey>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.LastUsed, opt => opt.Ignore());

            CreateMap<ApiKeyUpdateDto, ApiKey>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore()) 
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.LastUsed, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ApiKey, TenantConfigurationDto>()
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
                    src.Activo && (!src.FechaExpiracion.HasValue || src.FechaExpiracion > DateTime.UtcNow)));
        }
    }
}