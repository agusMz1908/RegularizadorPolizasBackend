﻿using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.DTOs.Audit;
using RegularizadorPolizas.Domain.Entities;
using RegularizadorPolizas.Domain.Enums;
using System.ComponentModel;
using System.Reflection;

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

            CreateMap<ProcessDocument, ProcessingDocumentDto>()
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

            CreateCompanyMappings();
            CreateBrokerMappings();
            CreateCurrencyMappings();
            CreateUserMappings();

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
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.No_utiles, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.No_utiles, opt => opt.Ignore())
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Company, CompanyLookupDto>();
            CreateMap<CompanyDto, CompanyLookupDto>();

            CreateMap<CompanyCreateDto, CompanyDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.No_utiles, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));

            CreateMap<CompanyCreateDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.No_utiles, opt => opt.Ignore())
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Comnom))
                .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.Comalias))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Cod_srvcompanias));

            CreateMap<Company, CompanySummaryDto>();
            CreateMap<CompanyDto, CompanySummaryDto>();

            CreateMap<CompanyDto, object>()
                .ConvertUsing<CompanyLegacyConverter>();
        }

        private void CreateBrokerMappings()
        {
            CreateMap<Broker, BrokerDto>()
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Broker, BrokerLookupDto>();
            CreateMap<BrokerDto, BrokerLookupDto>();

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
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Domicilio, opt => opt.MapFrom(src => src.Direccion));

            CreateMap<Broker, BrokerSummaryDto>();
            CreateMap<BrokerDto, BrokerSummaryDto>();

            CreateMap<BrokerDto, object>()
                .ConvertUsing<BrokerLegacyConverter>();
        }

        private void CreateCurrencyMappings()
        {
            CreateMap<Currency, CurrencyDto>()
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Currency, CurrencyLookupDto>();
            CreateMap<CurrencyDto, CurrencyLookupDto>();

            CreateMap<CurrencyCreateDto, CurrencyDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));

            CreateMap<CurrencyCreateDto, Currency>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Polizas, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Moneda));

            CreateMap<Currency, CurrencySummaryDto>();
            CreateMap<CurrencyDto, CurrencySummaryDto>();

            CreateMap<CurrencyDto, object>()
                .ConvertUsing<CurrencyLegacyConverter>();

            CreateMap<VelneoCurrencyDto, CurrencyDto>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Moneda)) 
                .ForMember(dest => dest.Simbolo, opt => opt.Ignore()) 
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.TotalPolizas, opt => opt.Ignore());
        }

        private void CreateUserMappings()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.RoleNames, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.ProcessDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.Renovations, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.ProcessDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.Renovations, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<User, UserSummaryDto>()
                .ForMember(dest => dest.TotalRoles, opt => opt.Ignore()) 
                .ForMember(dest => dest.PrimaryRole, opt => opt.Ignore()) 
                .ForMember(dest => dest.UltimaActividad, opt => opt.MapFrom(src => src.FechaModificacion))
                .ForMember(dest => dest.PuedeEliminar, opt => opt.Ignore()); 

            CreateMap<User, UserLookupDto>();

            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.TotalUsers, opt => opt.Ignore()) 
                .ForMember(dest => dest.TotalPermissions, opt => opt.Ignore()) 
                .ReverseMap()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            CreateMap<Role, RoleLookupDto>();

            CreateMap<UserRole, UserRoleAssignmentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Nombre))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.AssignedByName, opt => opt.MapFrom(src => src.AssignedByUser != null ? src.AssignedByUser.Nombre : "Sistema"));

            CreateMap<Permission, PermissionDto>()
                .ReverseMap();
        }

        private string GetAuditEventDescription(AuditEventType eventType)
        {
            var field = eventType.GetType().GetField(eventType.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? eventType.ToString();
        }
    }

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
                comnom = source.Comnom,
                comalias = source.Comalias,
                cod_srvcompanias = source.Cod_srvcompanias,
                broker = source.Broker,
                comrazsoc = source.Comrazsoc,
                comruc = source.Comruc
            };
        }
    }

    public class BrokerLegacyConverter : ITypeConverter<BrokerDto, object>
    {
        public object Convert(BrokerDto source, object destination, ResolutionContext context)
        {
            if (source == null) return null;

            return new
            {
                id = source.Id,
                nombre = source.Nombre, 
                codigo = source.Codigo,
                domicilio = source.Domicilio, 
                telefono = source.Telefono,
                email = source.Email,
                activo = source.Activo,
                totalPolizas = source.TotalPolizas,
                puedeEliminar = source.PuedeEliminar,
                name = source.Name,
                direccion = source.Direccion,
                observaciones = source.Observaciones,
                foto = source.Foto
            };
        }
    }
    public class CurrencyLegacyConverter : ITypeConverter<CurrencyDto, object>
    {
        public object Convert(CurrencyDto source, object destination, ResolutionContext context)
        {
            if (source == null) return null;

            return new
            {
                id = source.Id,
                moneda = source.Moneda,
                nombre = source.Nombre,
                simbolo = source.Simbolo,
                codigo = source.Codigo,
                activo = source.Activo,
                totalPolizas = source.TotalPolizas,
                puedeEliminar = source.PuedeEliminar
            };
        }
    }
}