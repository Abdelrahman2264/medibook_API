namespace medibook_API.Extensions.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public DateTime Expiration { get; set; }
    }
}
