namespace medibook_API.Extensions.DTOs
{
    public class ForgetPasswordDto
    {
        public int userId { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }
    }
}
