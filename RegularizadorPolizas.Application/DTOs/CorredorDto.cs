using System.Text.Json.Serialization;

public class CorredorDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("corrnom")]
    public string Corrnom { get; set; } = string.Empty;

    [JsonPropertyName("corrdom")]
    public string Corrdom { get; set; } = string.Empty;

    [JsonPropertyName("corrtel")]
    public string Corrtel { get; set; } = string.Empty;

    [JsonPropertyName("corrfax")]
    public string Corrfax { get; set; } = string.Empty;

    [JsonPropertyName("corremail")]
    public string Corremail { get; set; } = string.Empty;

    [JsonPropertyName("corrweb")]
    public string Corrweb { get; set; } = string.Empty;

    [JsonPropertyName("corrtelcel")]
    public string Corrtelcel { get; set; } = string.Empty;

    [JsonPropertyName("corrfoto")]
    public string Corrfoto { get; set; } = string.Empty;

    [JsonPropertyName("broker")]
    public bool Broker { get; set; }

    [JsonPropertyName("rut")]
    public string Rut { get; set; } = string.Empty;

    [JsonPropertyName("cod_corr")]
    public string Cod_corr { get; set; } = string.Empty;

    [JsonPropertyName("cod_organizador")]
    public string Cod_organizador { get; set; } = string.Empty;

    [JsonPropertyName("link_g_map")]
    public string Link_g_map { get; set; } = string.Empty;

    [JsonPropertyName("link_a_map")]
    public string Link_a_map { get; set; } = string.Empty;
}