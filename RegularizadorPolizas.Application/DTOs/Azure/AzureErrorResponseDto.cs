namespace RegularizadorPolizas.Application.DTOs.Azure
{
    public class AzureErrorResponseDto
    {
        public string Error { get; set; } = string.Empty;
        public string? Archivo { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public long TiempoProcesamiento { get; set; }
        public string Estado { get; set; } = "ERROR";
        public string? CodigoError { get; set; }
        public string? DetallesTecnicos { get; set; }
        public List<string>? Sugerencias { get; set; }
        public static AzureErrorResponseDto ArchivoInvalido(string? nombreArchivo = null) => new()
        {
            Error = "No se ha proporcionado un archivo válido",
            Archivo = nombreArchivo,
            CodigoError = "ARCHIVO_INVALIDO",
            Sugerencias = new List<string>
            {
                "Asegúrese de que el archivo sea un PDF válido",
                "Verifique que el archivo no esté corrupto",
                "El archivo debe tener contenido (no puede estar vacío)"
            }
        };

        public static AzureErrorResponseDto ErrorExtraccion(string mensaje, string? archivo = null) => new()
        {
            Error = $"Error en extracción de datos: {mensaje}",
            Archivo = archivo,
            CodigoError = "ERROR_EXTRACCION",
            Sugerencias = new List<string>
            {
                "Verifique que el documento sea una póliza de seguro válida",
                "Asegúrese de que el texto del PDF sea legible",
                "Intente con un archivo de mejor calidad"
            }
        };

        public static AzureErrorResponseDto ErrorBusquedaCliente(string mensaje, string? archivo = null) => new()
        {
            Error = $"Error en búsqueda de cliente: {mensaje}",
            Archivo = archivo,
            CodigoError = "ERROR_BUSQUEDA_CLIENTE",
            Sugerencias = new List<string>
            {
                "Verifique la conexión con la base de datos",
                "Los datos del cliente pueden requerir búsqueda manual",
                "Revise que el nombre y documento estén correctamente extraídos"
            }
        };

        public static AzureErrorResponseDto ErrorGeneral(string mensaje, string? archivo = null) => new()
        {
            Error = mensaje,
            Archivo = archivo,
            CodigoError = "ERROR_GENERAL",
            Sugerencias = new List<string>
            {
                "Intente procesar el documento nuevamente",
                "Verifique la conexión a internet",
                "Contacte al administrador si el problema persiste"
            }
        };
    }
}