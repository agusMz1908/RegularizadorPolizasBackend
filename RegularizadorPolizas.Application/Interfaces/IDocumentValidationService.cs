using RegularizadorPolizas.Application.DTOs;

namespace RegularizadorPolizas.Application.Interfaces
{
    public interface IDocumentValidationService
    {
        DocumentValidationResult ValidateExtractedData(PolizaDto poliza);
        DocumentValidationResult ValidateForCreation(PolizaDto poliza);
    }
}