using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Models
{
    public class VelneoDepartamento
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("dptnom")] 
        public string Dptnom { get; set; } = string.Empty;

        [JsonPropertyName("dptbonint")]  
        public string DptBonintString { get; set; } = "0";

        [JsonPropertyName("sc_cod")]  
        public string ScCod { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = true;

        public string Nombre => Dptnom;  
        public string CodigoSC => ScCod;  

        public decimal BonificacionInterior
        {
            get
            {
                return decimal.TryParse(DptBonintString, out var result) ? result : 0;
            }
        }
    }

    public class VelneoDepartamentosResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int Total_Count { get; set; }

        [JsonPropertyName("departamentos")]
        public List<VelneoDepartamento> Departamentos { get; set; } = new();
    }
}