public class CrearPolizaConClienteRequest
{
    public int ClienteId { get; set; }
    public DatosPolizaExtraidos DatosPoliza { get; set; } = new();
    public string ArchivoOriginal { get; set; } = string.Empty;
    public string ObservacionesUsuario { get; set; } = string.Empty;
    public bool ConfirmadoPorUsuario { get; set; }
}