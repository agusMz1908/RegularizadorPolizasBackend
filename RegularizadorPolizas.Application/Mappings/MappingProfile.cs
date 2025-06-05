using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Audit;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;
using System.ComponentModel;
using System.Reflection;
using static BrokerSummaryDto;

namespace RegularizadorPolizas.Application.Mappings
{
    public partial class MappingProfile : Profile
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

            // COMPANY MAPPINGS ACTUALIZADOS
            CreateCompanyMappings();

            // BROKER MAPPINGS ACTUALIZADOS
            CreateBrokerMappings();

            CreateMap<Currency, CurrencyDto>()
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore());

            CreateMap<Currency, CurrencyLookupDto>();

            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(dest => dest.EventTypeDescription, opt => opt.MapFrom(src => GetAuditEventDescription(src.EventType)))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.ToString()));
        }

        private void CreateCompanyMappings()
        {
            // Mapeo principal Company Entity <-> CompanyDto
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore()) // Se calcula en el servicio
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now));

            // Mapeo para Lookup
            CreateMap<Company, CompanyLookupDto>();
            CreateMap<CompanyDto, CompanyLookupDto>();

            // Mapeo para creación
            CreateMap<CompanyCreateDto, CompanyDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));

            CreateMap<CompanyCreateDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                // Sincronizar campos para compatibilidad
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Comnom))
                .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.Comalias))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Cod_srvcompanias));

            // Mapeo para resumen
            CreateMap<Company, CompanySummaryDto>();
            CreateMap<CompanyDto, CompanySummaryDto>();

            // Mapeo específico para compatibilidad hacia atrás
            CreateMap<CompanyDto, object>()
                .ConvertUsing<CompanyLegacyConverter>();

            // Mapeo desde Velneo
            CreateMap<VelneoCompanyDto, CompanyDto>()
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));
        }

        private void CreateBrokerMappings()
        {
            // Mapeo principal Broker Entity <-> BrokerDto
            CreateMap<Broker, BrokerDto>()
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore()) // Se calcula en el servicio
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now));

            // Mapeo para Lookup
            CreateMap<Broker, BrokerLookupDto>();
            CreateMap<BrokerDto, BrokerLookupDto>();

            // Mapeo para creación
            CreateMap<BrokerCreateDto, BrokerDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));

            CreateMap<BrokerCreateDto, Broker>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                // Sincronizar campos para compatibilidad
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Domicilio, opt => opt.MapFrom(src => src.Direccion));

            // Mapeo para resumen
            CreateMap<Broker, BrokerSummaryDto>();
            CreateMap<BrokerDto, BrokerSummaryDto>();

            // Mapeo específico para compatibilidad hacia atrás
            CreateMap<BrokerDto, object>()
                .ConvertUsing<BrokerLegacyConverter>();

            // Mapeo desde Velneo
            CreateMap<VelneoBrokerDto, BrokerDto>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore()) // No viene de Velneo
                .ForMember(dest => dest.Email, opt => opt.Ignore())  // No viene de Velneo
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore());
        }

        private string GetAuditEventDescription(AuditEventType eventType)
        {
            var field = eventType.GetType().GetField(eventType.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? eventType.ToString();
        }
    }

    // Converter para formato legacy de Company
    public class CompanyLegacyConverter : ITypeConverter<CompanyDto, object>
    {
        public object Convert(CompanyDto source, object destination, ResolutionContext context)
        {
            if (source == null) return null;

            return new
            {
                id = source.Id,
                nombre = source.Nombre,
                alias = source.Alias,
                codigo = source.Codigo,
                activo = source.Activo,
                totalPolizas = source.TotalPolizas,
                puedeEliminar = source.PuedeEliminar,
                // Campos adicionales de Velneo disponibles si se necesitan
                comnom = source.Comnom,
                comalias = source.Comalias,
                cod_srvcompanias = source.Cod_srvcompanias,
                broker = source.Broker,
                comrazsoc = source.Comrazsoc,
                comruc = source.Comruc
            };
        }
    }

    // Converter para formato legacy de Broker
    public class BrokerLegacyConverter : ITypeConverter<BrokerDto, object>
    {
        public object Convert(BrokerDto source, object destination, ResolutionContext context)
        {
            if (source == null) return null;

            return new
            {
                id = source.Id,
                nombre = source.Nombre, // Campo de compatibilidad
                codigo = source.Codigo,
                domicilio = source.Domicilio, // Campo de compatibilidad
                telefono = source.Telefono,
                email = source.Email,
                activo = source.Activo,
                totalPolizas = source.TotalPolizas,
                puedeEliminar = source.PuedeEliminar,
                // Campos adicionales de Velneo disponibles si se necesitan
                name = source.Name,
                direccion = source.Direccion,
                observaciones = source.Observaciones,
                foto = source.Foto
            };
        }
    }
}