namespace medibook_API.Extensions.DTOs
{
    public class EmailSettingsDto
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
