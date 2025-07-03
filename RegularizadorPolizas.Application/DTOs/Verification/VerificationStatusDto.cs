namespace RegularizadorPolizas.Application.DTOs.Verification
{
    public class VerificationStatusDto
    {
        public string PolizaId { get; set; }
        public int UserId { get; set; }
        public string NombreUsuario { get; set; }
        public Dictionary<string, VerificationFieldDto> CamposVerificados { get; set; } = new Dictionary<string, VerificationFieldDto>();
        public string EstadoGeneral { get; set; }
        public DateTime FechaVerificacion { get; set; }
        public string Observaciones { get; set; }
        public int CamposVerificadosCount { get; set; }
        public int CamposConErroresCount { get; set; }
        public decimal PorcentajeCompletado { get; set; }
    }

    public class VerificationFieldDto
    {
        public string NombreCampo { get; set; }
        public string EtiquetaCampo { get; set; }
        public string ValorExtraido { get; set; }
        public string ValorVelneo { get; set; }
        public bool EsVerificado { get; set; }
        public bool TieneDiscrepancia { get; set; }
        public string ComentarioUsuario { get; set; }
        public string ValorCorregido { get; set; }
        public decimal ConfianzaExtraccion { get; set; }
        public bool EsCampoObligatorio { get; set; }
        public string TipoDato { get; set; }
    }
}