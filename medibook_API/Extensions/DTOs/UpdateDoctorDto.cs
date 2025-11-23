namespace medibook_API.Extensions.DTOs
{
    public class UpdateDoctorDto
    {

        public string? Bio { get; set; }
        public string? Specialization { get; set; }
        public string? Type { get; set; }
        public int? ExperienceYears { get; set; }

        // User fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobilePhone { get; set; }
        public byte[]? profile_image { get; set; }
    }
}
