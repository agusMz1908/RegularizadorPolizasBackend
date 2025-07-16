namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class AzureBatchResponseDto
    {
        public int Procesados { get; set; }
        public int Errores { get; set; }
        public int TotalArchivos { get; set; }
        public List<AzureProcessResponseDto> Resultados { get; set; } = new();
        public List<AzureBatchErrorDto> ErroresDetalle { get; set; } = new();
        public DateTime FechaProcesamiento { get; set; } = DateTime.UtcNow;
        public long TiempoTotalProcesamiento { get; set; }
        public double PorcentajeExito => TotalArchivos > 0 ? (Procesados / (double)TotalArchivos) * 100 : 0;
        public bool TodosExitosos => Errores == 0;
        public bool AlgunosExitosos => Procesados > 0;
        public double TiempoPromedioPorArchivo => Procesados > 0 ? TiempoTotalProcesamiento / (double)Procesados : 0;

        public AzureBatchEstadisticasDto Estadisticas => new()
        {
            TotalProcesados = Procesados,
            TotalErrores = Errores,
            PorcentajeExito = PorcentajeExito,
            ClientesEncontrados = Resultados.Count(r => r.BusquedaCliente.TieneMatches),
            ListosParaVelneo = Resultados.Count(r => r.ListoParaVelneo),
            RequierenIntervencion = Resultados.Count(r => r.RequiereIntervencion)
        };
    }

    public class AzureBatchErrorDto
    {
        public string Archivo { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? CodigoError { get; set; }
        public string? DetallesTecnicos { get; set; }
        public bool EsErrorDeArchivo => Error.Contains("archivo", StringComparison.OrdinalIgnoreCase);
        public bool EsErrorDeRed => Error.Contains("conexión", StringComparison.OrdinalIgnoreCase) ||
                                   Error.Contains("timeout", StringComparison.OrdinalIgnoreCase);
        public bool EsErrorDeAzure => Error.Contains("azure", StringComparison.OrdinalIgnoreCase);
    }

    public class AzureBatchEstadisticasDto
    {
        public int TotalProcesados { get; set; }
        public int TotalErrores { get; set; }
        public double PorcentajeExito { get; set; }
        public int ClientesEncontrados { get; set; }
        public int ListosParaVelneo { get; set; }
        public int RequierenIntervencion { get; set; }

        public string ResumenTexto => $"{TotalProcesados} procesados, {ClientesEncontrados} con clientes, {ListosParaVelneo} listos para Velneo";
    }
}