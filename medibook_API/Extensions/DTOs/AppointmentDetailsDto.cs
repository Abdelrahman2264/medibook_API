public class AppointmentDetailsDto
{
    // Existing properties...
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string PatientFirstName { get; set; }
    public string PatientLastName { get; set; }
    public string PatientGender { get; set; }
    public string PatientMartialStatus { get; set; }
    public string PatientMobilePhone { get; set; }
    public int DoctorId { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public string DoctorGender { get; set; }
    public string DoctorMobilePhone { get; set; }
    public string DoctorType { get; set; }
    public string DoctorSpecialization { get; set; }

    // Change these to nullable int
    public int? NurseId { get; set; }  // Changed from int to int?
    public string NurseFirstName { get; set; }
    public string NurseLastName { get; set; }
    public string NurseGender { get; set; }

    public int? RoomId { get; set; }   // Changed from int to int?
    public string RoomName { get; set; }
    public string RoomType { get; set; }

    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string Medicine { get; set; }
    public string Notes { get; set; }
}