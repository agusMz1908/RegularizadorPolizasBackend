using RegularizadorPolizas.Application.DTOs;

public interface IVelneoMapperService
{
    Task<object> MapearCreateRequestAsync(PolizaCreateRequest request);
    void ValidarCamposRequeridos(PolizaCreateRequest request);
    string FormatearFecha(object? fecha);
    int? TryParseInt(string? value);
}