using RegularizadorPolizas.Application.DTOs;
using RegularizadorPolizas.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            if (string.IsNullOrWhiteSpace(poliza.Confchdes))
                result.Warnings.Add("Fecha de inicio de vigencia no encontrada");

            if (string.IsNullOrWhiteSpace(poliza.Confchhas))
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
            ValidateDateCoherence(poliza, result);
            ValidateAmountCoherence(poliza, result);
            ValidateDocumentFormat(poliza, result);
        }

        private void ValidateDateCoherence(PolizaDto poliza, DocumentValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(poliza.Confchdes) || string.IsNullOrWhiteSpace(poliza.Confchhas))
                return; 

            var fechaInicio = TryParseVelneoDate(poliza.Confchdes);
            var fechaFin = TryParseVelneoDate(poliza.Confchhas);

            if (fechaInicio == null)
            {
                result.Warnings.Add($"Fecha de inicio de vigencia tiene formato inválido: '{poliza.Confchdes}'");
                return;
            }

            if (fechaFin == null)
            {
                result.Warnings.Add($"Fecha de fin de vigencia tiene formato inválido: '{poliza.Confchhas}'");
                return;
            }

            if (fechaInicio >= fechaFin)
            {
                result.Errors.Add("La fecha de inicio de vigencia debe ser anterior a la fecha de fin");
            }

            var diferenciaDias = (fechaFin.Value - fechaInicio.Value).TotalDays;

            if (diferenciaDias > 400)
                result.Warnings.Add("La vigencia de la póliza parece ser muy larga (más de 400 días)");

            if (diferenciaDias < 30) 
                result.Warnings.Add("La vigencia de la póliza parece ser muy corta (menos de 30 días)");

            if (fechaInicio > DateTime.Now.AddYears(2))
                result.Warnings.Add("La fecha de inicio está muy en el futuro");

            if (fechaFin < DateTime.Now.AddYears(-5))
                result.Warnings.Add("La póliza parece estar muy vencida");
        }

        private void ValidateAmountCoherence(PolizaDto poliza, DocumentValidationResult result)
        {
            if (poliza.Conpremio.HasValue && poliza.Contot.HasValue)
            {
                if (poliza.Conpremio > poliza.Contot)
                    result.Warnings.Add("La prima comercial es mayor que el premio total, esto puede ser incorrecto");

                if (poliza.Conpremio <= 0)
                    result.Errors.Add("La prima comercial debe ser mayor a 0");

                if (poliza.Contot <= 0)
                    result.Errors.Add("El premio total debe ser mayor a 0");

                if (poliza.Conpremio > 1000000)
                    result.Warnings.Add("La prima comercial parece muy alta, verificar si es correcta");

                if (poliza.Contot > 1000000)
                    result.Warnings.Add("El premio total parece muy alto, verificar si es correcto");
            }
        }

        private void ValidateDocumentFormat(PolizaDto poliza, DocumentValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(poliza.Cliruc))
                return;

            var documento = poliza.Cliruc.Replace(".", "").Replace("-", "").Replace(" ", "").Trim();

            if (documento.Length < 7 || documento.Length > 12)
            {
                result.Warnings.Add("El formato del documento parece ser inválido (longitud incorrecta)");
                return;
            }

            if (documento.Length == 8 && !documento.All(char.IsDigit))
            {
                result.Warnings.Add("La cédula de identidad debe contener solo números");
            }

            if (documento.Length == 12 && !documento.All(char.IsDigit))
            {
                result.Warnings.Add("El RUT debe contener solo números");
            }
        }

        private DateTime? TryParseVelneoDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            string[] formatos = {
                "yyyy-MM-dd",         
                "dd/MM/yyyy",           
                "MM/dd/yyyy",       
                "dd-MM-yyyy",           
                "yyyy/MM/dd",           
                "yyyyMMdd",             
                "dd.MM.yyyy",         
                "yyyy-MM-ddTHH:mm:ss",  
                "yyyy-MM-dd HH:mm:ss"  
            };

            foreach (var formato in formatos)
            {
                if (DateTime.TryParseExact(dateString.Trim(), formato,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    return fecha;
                }
            }

            if (DateTime.TryParse(dateString.Trim(), out var fechaGeneral))
            {
                return fechaGeneral;
            }

            return null;
        }

        public DocumentValidationResult ValidateForCreation(PolizaDto poliza)
        {
            var result = ValidateExtractedData(poliza);

            if (string.IsNullOrWhiteSpace(poliza.Condom))
                result.Warnings.Add("Dirección del asegurado no encontrada");

            if (string.IsNullOrWhiteSpace(poliza.Clidptnom))
                result.Warnings.Add("Departamento del asegurado no encontrado");

            if (!poliza.Comcod.HasValue || poliza.Comcod <= 0)
                result.Errors.Add("Código de compañía requerido para creación en Velneo");

            if (!poliza.Clinro.HasValue || poliza.Clinro <= 0)
                result.Errors.Add("Código de cliente requerido para creación en Velneo");

            if (!poliza.Seccod.HasValue || poliza.Seccod <= 0)
                result.Warnings.Add("Código de sección no encontrado, se usará valor por defecto");

            if (!string.IsNullOrWhiteSpace(poliza.Confchdes))
            {
                var fechaInicio = TryParseVelneoDate(poliza.Confchdes);
                if (fechaInicio == null)
                    result.Errors.Add("Fecha de inicio debe tener formato válido para Velneo (yyyy-MM-dd recomendado)");
            }

            if (!string.IsNullOrWhiteSpace(poliza.Confchhas))
            {
                var fechaFin = TryParseVelneoDate(poliza.Confchhas);
                if (fechaFin == null)
                    result.Errors.Add("Fecha de fin debe tener formato válido para Velneo (yyyy-MM-dd recomendado)");
            }

            return result;
        }

        public DocumentValidationResult ValidateAzureExtraction(PolizaDto poliza)
        {
            var result = ValidateExtractedData(poliza);

            if (string.IsNullOrWhiteSpace(poliza.Conpol))
                result.Errors.Add("Azure no pudo extraer el número de póliza del documento");

            if (string.IsNullOrWhiteSpace(poliza.Clinom) && string.IsNullOrWhiteSpace(poliza.Cliruc))
                result.Errors.Add("Azure no pudo extraer información básica del asegurado");

            var camposExtraidos = 0;
            var camposImportantes = new[] {
                poliza.Conpol, poliza.Clinom, poliza.Cliruc, poliza.Conmaraut,
                poliza.Conmataut, poliza.Confchdes, poliza.Confchhas
            };

            camposExtraidos = camposImportantes.Count(campo => !string.IsNullOrWhiteSpace(campo));

            if (camposExtraidos < 4)
                result.Warnings.Add($"Azure extrajo pocos campos importantes ({camposExtraidos}/7). " +
                    "La calidad del documento puede ser baja.");

            return result;
        }
    }
}