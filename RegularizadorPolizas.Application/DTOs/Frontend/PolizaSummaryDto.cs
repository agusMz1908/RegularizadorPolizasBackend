namespace RegularizadorPolizas.Application.DTOs.Frontend
{
    public class PolizaSummaryDto
    {
        public int Id { get; set; }
        public string Numero { get; set; }        // PolizaDto.Conpol
        public DateTime? Desde { get; set; }      // PolizaDto.Confchdes
        public DateTime? Hasta { get; set; }      // PolizaDto.Confchhas
        public string Compania { get; set; }      // Mapear desde PolizaDto.Comcod
        public string Ramo { get; set; }          // Mapear desde PolizaDto.Seccod
        public string Estado { get; set; }        // Calculado (Nuevo, Vigente, Vencido, Cancelado)
        public string Certificado { get; set; }   // PolizaDto.Concertif
        public decimal? Prima { get; set; }       // PolizaDto.Conpremio
        public string Moneda { get; set; }        // Mapear desde PolizaDto.Moncod
        public bool RequiereAtencion { get; set; } // Calculado
        public string TipoOperacion { get; set; } // Calculado (Nueva, Renovación, Endoso)

        public string Vigencia { get; set; }      // PolizaDto.Convig
        public string Endoso { get; set; }        // PolizaDto.Conend
        public decimal? Total { get; set; }       // PolizaDto.Contot
    }
}