namespace RegularizadorPolizas.Application.DTOs.Auth
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Nombre { get; set; }
        public string TenantId { get; set; }
        public DateTime Expiration { get; set; }
    }
}