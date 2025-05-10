namespace RegularizadorPolizas.Application.DTOs.Auth
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public int ClienteId { get; set; }
        public int? UsuarioId { get; set; }
        public string Nombre { get; set; }
        public DateTime Expiration { get; set; }
    }
}