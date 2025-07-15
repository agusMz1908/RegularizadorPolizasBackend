using RegularizadorPolizas.Application.DTOs;

public class ClienteMatchResult
{
    public List<ClienteMatch> Matches { get; set; } = new();
    public TipoResultadoCliente TipoResultado { get; set; }
    public string MensajeUsuario { get; set; } = string.Empty;
    public bool RequiereIntervencionManual { get; set; }
    public DatosClienteExtraidos DatosOriginales { get; set; } = new();

    public enum TipoResultadoCliente
    {
        MatchExacto,       
        MatchMuyProbable, 
        MultiplesMatches,  
        MatchParcial,      
        SinCoincidencias   
    }
}