namespace medibook_API.Extensions.DTOs
{
    public class DoctorDetailsDto
    {
        // User fields
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Gender { get; set; }
        public byte[]? ProfileImage { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Doctor fields
        public int DoctorId { get; set; }
        public string Bio { get; set; }
        public string Specialization { get; set; }
        public string Type { get; set; }
        public int ExperienceYears { get; set; }
        public bool IsActive { get; set; }

        // Optional metadata
        public DateTime CreateDate { get; set; }


    }
}
