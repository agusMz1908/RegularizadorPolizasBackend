namespace RegularizadorPolizas.Application.DTOs.Verification
{
    public class PolizaCorrectionDto
    {
        public string PolizaId { get; set; }
        public int UserId { get; set; }
        public Dictionary<string, string> CamposCorregidos { get; set; } = new Dictionary<string, string>();
        public string MotivoCorreccion { get; set; }
        public bool ReenviarAVelneo { get; set; }
        public string Prioridad { get; set; } 
        public List<string> CamposCriticos { get; set; } = new List<string>();
        public DateTime FechaCorreccion { get; set; } = DateTime.Now;
    }
}