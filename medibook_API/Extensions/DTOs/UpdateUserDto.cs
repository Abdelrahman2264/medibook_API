namespace medibook_API.Extensions.DTOs
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobilePhone { get; set; }
        public string? Gender { get; set; }
        public string? MitrialStatus { get; set; }
        public byte[]? ProfileImage { get; set; }

    }
}
