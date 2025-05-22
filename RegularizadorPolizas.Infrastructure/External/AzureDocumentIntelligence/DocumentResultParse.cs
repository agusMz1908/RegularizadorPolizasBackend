using System.Globalization;
using System.Text.RegularExpressions;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using RegularizadorPolizas.Application.DTOs;

public class DocumentResultParser
{
    public PolizaDto ParseToPolizaDto(AnalyzeResult analyzeResult)
    {
        if (analyzeResult?.Documents == null || !analyzeResult.Documents.Any())
            return null;

        var poliza = new PolizaDto();
        var document = analyzeResult.Documents.FirstOrDefault();

        if (document?.Fields == null)
            return poliza;

        var fields = document.Fields;

        MapearDatosBasicos(poliza, fields);
        MapearDatosAsegurado(poliza, fields);
        MapearDatosVehiculo(poliza, fields);
        MapearDatosFinancieros(poliza, fields);
        MapearDatosCorredor(poliza, fields);

        return poliza;
    }

    private void MapearDatosBasicos(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
    {
        ExtraerCampo(fields, "poliza.numero", valor => poliza.Conpol = valor);
        ExtraerCampo(fields, "poliza.endoso", valor => poliza.Conend = valor);
        ExtraerCampo(fields, "poliza.ramo", valor => poliza.Ramo = valor);
        ExtraerCampo(fields, "poliza.producto", valor => poliza.Ramo = valor);
        ExtraerCampo(fields, "poliza.tipo_renovacion", valor => poliza.Congesti = valor);
        ExtraerCampo(fields, "poliza.moneda", valor => {
            if (valor?.ToLower().Contains("peso") == true)
                poliza.Moncod = 1;
            else if (valor?.ToLower().Contains("dolar") == true)
                poliza.Moncod = 2;
        });

        ExtraerFecha(fields, "poliza.vigencia.desde", fecha => poliza.Confchdes = fecha);
        ExtraerFecha(fields, "poliza.vigencia.hasta", fecha => poliza.Confchhas = fecha);
        ExtraerFecha(fields, "poliza.fecha_emision", fecha => poliza.FechaCreacion = fecha);
    }

    private void MapearDatosAsegurado(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
    {
        ExtraerCampo(fields, "asegurado.nombre", valor => poliza.Clinom = valor);
        ExtraerCampo(fields, "asegurado.direccion", valor => poliza.Condom = valor);
        ExtraerCampo(fields, "asegurado.localidad", valor => poliza.Clilocnom = valor);
        ExtraerCampo(fields, "asegurado.departamento", valor => poliza.Clidptnom = valor);
        ExtraerCampo(fields, "asegurado.documento.numero", valor => poliza.Cliruc = valor);
        ExtraerCampo(fields, "asegurado.radio", valor => poliza.Cliposcod = int.TryParse(valor, out int radio) ? radio : null);
    }

    private void MapearDatosVehiculo(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
    {
        ExtraerCampo(fields, "vehiculo.marca", valor => poliza.Conmaraut = valor);
        ExtraerCampo(fields, "vehiculo.tipo", valor => poliza.Contpocob = valor);
        ExtraerCampo(fields, "vehiculo.modelo", valor => poliza.Condetail = valor);
        ExtraerCampo(fields, "vehiculo.matricula", valor => poliza.Conmataut = valor);
        ExtraerCampo(fields, "vehiculo.motor", valor => poliza.Conmotor = valor);
        ExtraerCampo(fields, "vehiculo.padron", valor => poliza.Conpadaut = valor);
        ExtraerCampo(fields, "vehiculo.combustible", valor => poliza.Combustibles = valor);
        ExtraerCampo(fields, "vehiculo.año", valor => {
            if (int.TryParse(valor, out int anio))
                poliza.Conanioaut = anio;
        });
    }

    private void MapearDatosFinancieros(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
    {
        ExtraerDecimal(fields, "financiero.prima_comercial", valor => poliza.Conpremio = valor);
        ExtraerDecimal(fields, "financiero.premio_total", valor => poliza.Contot = valor);
        ExtraerDecimal(fields, "financiero.impuesto_msp", valor => poliza.Conimp = valor);

        ExtraerCampo(fields, "pago.medio", valor => poliza.Consta = valor);
        ExtraerCampo(fields, "pago.modo_facturacion", valor => {
            var match = Regex.Match(valor ?? "", @"(\d+)\s*cuotas?");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int cuotas))
                poliza.Concuo = cuotas;
        });
    }

    private void MapearDatosCorredor(PolizaDto poliza, IReadOnlyDictionary<string, DocumentField> fields)
    {
        ExtraerCampo(fields, "corredor.nombre", valor => poliza.Com_alias = valor);
        ExtraerCampo(fields, "corredor.numero", valor => {
            if (int.TryParse(valor, out int numero))
                poliza.Corrnom = numero;
        });
    }

    #region Métodos Helper

    private void ExtraerCampo(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, Action<string> asignar)
    {
        if (fields.TryGetValue(fieldName, out var field) &&
            !string.IsNullOrWhiteSpace(field.Content))
        {
            asignar(field.Content.Trim());
        }
    }

    private void ExtraerFecha(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, Action<DateTime> asignar)
    {
        ExtraerCampo(fields, fieldName, valor => {
            if (DateTime.TryParse(valor, out DateTime fecha))
                asignar(fecha);
        });
    }

    private void ExtraerDecimal(IReadOnlyDictionary<string, DocumentField> fields, string fieldName, Action<decimal> asignar)
    {
        ExtraerCampo(fields, fieldName, valor => {
            string valorLimpio = valor.Replace("$", "").Replace(",", "").Replace(".", ",").Trim();
            if (decimal.TryParse(valorLimpio, NumberStyles.Any, new CultureInfo("es-UY"), out decimal resultado))
                asignar(resultado);
        });
    }

    internal PolizaDto ParseToPolizaDto(DocumentResultDto documento)
    {
        throw new NotImplementedException();
    }

    #endregion
}