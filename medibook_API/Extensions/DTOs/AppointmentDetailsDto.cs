using System.Text.Json.Serialization;

public class AppointmentDetailsDto
{
    // Existing properties...
    [JsonPropertyName("appointmentId")]
    public int AppointmentId { get; set; }
    [JsonPropertyName("patientId")]

    public int PatientId { get; set; }
    [JsonPropertyName("patientFirstName")]

    public string PatientFirstName { get; set; }
    [JsonPropertyName("patientLastName")]

    public string PatientLastName { get; set; }
    [JsonPropertyName("patientGender")]

    public string PatientGender { get; set; }
    [JsonPropertyName("patientMartialStatus")]

    public string PatientMartialStatus { get; set; }
    [JsonPropertyName("patientMobilePhone")]

    public string PatientMobilePhone { get; set; }
    [JsonPropertyName("doctorId")]

    public int DoctorId { get; set; }
    [JsonPropertyName("doctorFirstName")]

    public string DoctorFirstName { get; set; }
    [JsonPropertyName("doctorLastName")]


    public string DoctorLastName { get; set; }
    [JsonPropertyName("doctorGender")]

    public string DoctorGender { get; set; }
    [JsonPropertyName("doctorMobilePhone")]

    public string DoctorMobilePhone { get; set; }
    [JsonPropertyName("doctorType")]

    public string DoctorType { get; set; }
    [JsonPropertyName("doctorSpecialization")]

    public string DoctorSpecialization { get; set; }

    // Change these to nullable int
    [JsonPropertyName("nurseId")]

    public int? NurseId { get; set; }  // Changed from int to int?
    [JsonPropertyName("nurseFirstName")]

    public string NurseFirstName { get; set; }
    [JsonPropertyName("nurseLastName")]

    public string NurseLastName { get; set; }
    [JsonPropertyName("nurseGender")]

    public string NurseGender { get; set; }
    [JsonPropertyName("roomId")]

    public int? RoomId { get; set; }   // Changed from int to int?
    [JsonPropertyName("roomName")]

    public string RoomName { get; set; }
    [JsonPropertyName("roomType")]

    public string RoomType { get; set; }
    [JsonPropertyName("appointmentDate")]


    public DateTime AppointmentDate { get; set; }
    [JsonPropertyName("status")]

    public string Status { get; set; }
    [JsonPropertyName("medicine")]

    public string Medicine { get; set; }
    [JsonPropertyName("notes")]

    public string Notes { get; set; }
}