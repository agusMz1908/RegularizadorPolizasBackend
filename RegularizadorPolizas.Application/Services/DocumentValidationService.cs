using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RegularizadorPolizas.Application.Services
{
    public class DocumentValidationService : IDocumentValidationService
    {
        public DocumentValidationResult ValidateExtractedData(PolizaDto poliza)
        {
            var result = new DocumentValidationResult();
            ValidateCriticalFields(poliza, result);
            ValidateWarningFields(poliza, result);
            ValidateDataCoherence(poliza, result);

            result.IsValid = !result.Errors.Any();
            result.RequiresReview = result.Warnings.Any() || result.Errors.Any();

            return result;
        }

        private void ValidateCriticalFields(PolizaDto poliza, DocumentValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(poliza.Conpol))
                result.Errors.Add("Número de póliza no encontrado o vacío");

            if (string.IsNullOrWhiteSpace(poliza.Clinom))
                result.Errors.Add("Nombre del asegurado no encontrado o vacío");

            if (string.IsNullOrWhiteSpace(poliza.Cliruc))
                result.Errors.Add("Documento del asegurado no encontrado o vacío");

            if (string.IsNullOrWhiteSpace(poliza.Conmaraut))
                result.Errors.Add("Marca del vehículo no encontrada o vacía");

            if (string.IsNullOrWhiteSpace(poliza.Conmataut))
                result.Errors.Add("Matrícula del vehículo no encontrada o vacía");
        }

        private void ValidateWarningFields(PolizaDto poliza, DocumentValidationResult result)
        {
            if (!poliza.Confchdes.HasValue)
                result.Warnings.Add("Fecha de inicio de vigencia no encontrada");

            if (!poliza.Confchhas.HasValue)
                result.Warnings.Add("Fecha de fin de vigencia no encontrada");

            if (!poliza.Conpremio.HasValue || poliza.Conpremio <= 0)
                result.Warnings.Add("Prima comercial no encontrada o con valor inválido");

            if (!poliza.Contot.HasValue || poliza.Contot <= 0)
                result.Warnings.Add("Premio total no encontrado o con valor inválido");

            if (string.IsNullOrWhiteSpace(poliza.Conmotor))
                result.Warnings.Add("Número de motor no encontrado");

            if (string.IsNullOrWhiteSpace(poliza.Conchasis))
                result.Warnings.Add("Número de chasis no encontrado");

            if (!poliza.Conanioaut.HasValue || poliza.Conanioaut < 1900 || poliza.Conanioaut > DateTime.Now.Year + 1)
                result.Warnings.Add("Año del vehículo no encontrado o con valor inválido");

            if (string.IsNullOrWhiteSpace(poliza.Com_alias))
                result.Warnings.Add("Corredor no encontrado");
        }

        private void ValidateDataCoherence(PolizaDto poliza, DocumentValidationResult result)
        {
            // Validar coherencia de fechas
            if (poliza.Confchdes.HasValue && poliza.Confchhas.HasValue)
            {
                if (poliza.Confchdes >= poliza.Confchhas)
                    result.Errors.Add("La fecha de inicio de vigencia debe ser anterior a la fecha de fin");

                //var diferenciaDias = (poliza.Confchhas.Value - poliza.Confchdes.Value).TotalDays;
                //if (diferenciaDias > 400) // Más de un año y un mes aproximadamente
                //    result.Warnings.Add("La vigencia de la póliza parece ser muy larga");
                //if (diferenciaDias < 30) // Menos de un mes
                //    result.Warnings.Add("La vigencia de la póliza parece ser muy corta");
            }

            if (poliza.Conpremio.HasValue && poliza.Contot.HasValue)
            {
                if (poliza.Conpremio > poliza.Contot)
                    result.Warnings.Add("La prima comercial es mayor que el premio total, esto puede ser incorrecto");
            }

            if (!string.IsNullOrWhiteSpace(poliza.Cliruc))
            {
                var documento = poliza.Cliruc.Replace(".", "").Replace("-", "").Trim();
                if (documento.Length < 7 || documento.Length > 12)
                    result.Warnings.Add("El formato del documento parece ser inválido");
            }
        }

        public DocumentValidationResult ValidateForCreation(PolizaDto poliza)
        {
            var result = ValidateExtractedData(poliza);

            if (string.IsNullOrWhiteSpace(poliza.Condom))
                result.Warnings.Add("Dirección del asegurado no encontrada");

            if (string.IsNullOrWhiteSpace(poliza.Clidptnom))
                result.Warnings.Add("Departamento del asegurado no encontrado");

            return result;
        }
    }
}