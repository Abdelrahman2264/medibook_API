namespace medibook_API.Extensions.DTOs
{
    public class NurseDetailsDto
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
        public int NurseId { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; }

        // Optional metadata
        public DateTime CreateDate { get; set; }


    }
}
