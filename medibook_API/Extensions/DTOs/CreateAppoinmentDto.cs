namespace medibook_API.Extensions.DTOs
{
    public class CreateAppoinmentDto
    {
        public int patient_id { get; set; }
        public int doctor_id { get; set; }
        public DateTime appointment_date { get; set; }
    }
}
