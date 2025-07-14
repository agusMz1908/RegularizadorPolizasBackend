using AutoMapper;
using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Domain.Entities;

namespace RegularizadorPolizas.Application.Mappings
{
    public class ProcessDocumentMappingProfile : Profile
    {
        public ProcessDocumentMappingProfile()
        {
            CreateMap<ProcessDocument, ProcessingDocumentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.NombreArchivo))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.RutaArchivo))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.TipoDocumento))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.EstadoProcesamiento))
                .ForMember(dest => dest.CurrentStage, opt => opt.MapFrom(src => MapCurrentStage(src)))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompaniaId))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.CodigoCompania))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.Nombre : ""))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.FechaInicioProcesamiento))
                .ForMember(dest => dest.FinishTime, opt => opt.MapFrom(src => src.FechaFinProcesamiento))
                .ForMember(dest => dest.ModifiedTime, opt => opt.MapFrom(src => src.FechaModificacion))
                .ForMember(dest => dest.ProcessingTime, opt => opt.MapFrom(src => src.TiempoProcessamiento))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.CostoProcessamiento))
                .ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.NumeroPaginas))
                .ForMember(dest => dest.ConfidenceLevel, opt => opt.MapFrom(src => src.NivelConfianza))
                .ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.MensajeError))
                .ForMember(dest => dest.SentToVelneo, opt => opt.MapFrom(src => src.EnviadoVelneo))
                .ForMember(dest => dest.VelneoSentDate, opt => opt.MapFrom(src => src.FechaEnvioVelneo))
                .ForMember(dest => dest.VelneoResponse, opt => opt.MapFrom(src => src.RespuestaVelneo))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.TamanoArchivo))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.TipoMime))
                .ForMember(dest => dest.FileHash, opt => opt.MapFrom(src => src.HashArchivo))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Prioridad))
                .ForMember(dest => dest.ProcessingAttempts, opt => opt.MapFrom(src => src.IntentosProcesamiento))
                .ForMember(dest => dest.MaxAttempts, opt => opt.MapFrom(src => src.MaxIntentos))

                .ForMember(dest => dest.PolicyId, opt => opt.MapFrom(src => src.PolizaId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UsuarioId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Nombre : ""))
                .ForMember(dest => dest.PolicyNumber, opt => opt.MapFrom(src => src.Poliza != null ? src.Poliza.Conpol : ""));

            CreateMap<ProcessingDocumentDto, ProcessDocument>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ConvertStringToInt(src.Id)))
                .ForMember(dest => dest.NombreArchivo, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.RutaArchivo, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dest => dest.TipoDocumento, opt => opt.MapFrom(src => src.DocumentType))
                .ForMember(dest => dest.EstadoProcesamiento, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CodigoCompania, opt => opt.MapFrom(src => src.CompanyCode))
                .ForMember(dest => dest.CompaniaId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.CreationTime))
                .ForMember(dest => dest.FechaInicioProcesamiento, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.FechaFinProcesamiento, opt => opt.MapFrom(src => src.FinishTime))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.ModifiedTime))
                .ForMember(dest => dest.TiempoProcessamiento, opt => opt.MapFrom(src => src.ProcessingTime))
                .ForMember(dest => dest.CostoProcessamiento, opt => opt.MapFrom(src => src.Cost))
                .ForMember(dest => dest.NumeroPaginas, opt => opt.MapFrom(src => src.PageCount))
                .ForMember(dest => dest.NivelConfianza, opt => opt.MapFrom(src => src.ConfidenceLevel))
                .ForMember(dest => dest.MensajeError, opt => opt.MapFrom(src => src.ErrorMessage))
                .ForMember(dest => dest.EnviadoVelneo, opt => opt.MapFrom(src => src.SentToVelneo))
                .ForMember(dest => dest.FechaEnvioVelneo, opt => opt.MapFrom(src => src.VelneoSentDate))
                .ForMember(dest => dest.RespuestaVelneo, opt => opt.MapFrom(src => src.VelneoResponse))
                .ForMember(dest => dest.TamanoArchivo, opt => opt.MapFrom(src => src.FileSize))
                .ForMember(dest => dest.TipoMime, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.HashArchivo, opt => opt.MapFrom(src => src.FileHash))
                .ForMember(dest => dest.Prioridad, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.IntentosProcesamiento, opt => opt.MapFrom(src => src.ProcessingAttempts))
                .ForMember(dest => dest.MaxIntentos, opt => opt.MapFrom(src => src.MaxAttempts))
                .ForMember(dest => dest.PolizaId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Poliza, opt => opt.Ignore())
                .ForMember(dest => dest.ResultadoJson, opt => opt.Ignore())
                .ForMember(dest => dest.MetadatosJson, opt => opt.Ignore())
                .ForMember(dest => dest.FechaProcesamiento, opt => opt.Ignore());
        }

        private static string MapCurrentStage(ProcessDocument document)
        {
            if (document.FechaInicioProcesamiento.HasValue && !document.FechaFinProcesamiento.HasValue)
                return "azure_processing";
            if (document.EstadoProcesamiento == "COMPLETADO" && document.EnviadoVelneo != true)
                return "data_mapping";
            if (document.EnviadoVelneo == true)
                return "velneo_sending";
            if (document.EstadoProcesamiento == "ERROR")
                return "error";
            if (document.EstadoProcesamiento == "PROCESANDO")
                return "processing";
            return "pending";
        }

        private static int ConvertStringToInt(string value)
        {
            if (int.TryParse(value, out var result))
                return result;
            return 0;
        }
    }
}