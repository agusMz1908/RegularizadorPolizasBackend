using RegularizadorPolizas.Application.DTOs;

public class ClienteMatch
{
    public ClientDto Cliente { get; set; } = new();
    public int Score { get; set; } 
    public string Criterio { get; set; } = string.Empty; 
    public List<string> Coincidencias { get; set; } = new();
    public List<string> Diferencias { get; set; } = new();
}