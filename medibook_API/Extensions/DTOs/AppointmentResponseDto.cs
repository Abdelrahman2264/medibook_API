namespace medibook_API.Extensions.DTOs
{
    public class AppointmentResponseDto
    {
        public int appointment_id { get; set; }
        public DateTime appointment_date { get; set; }

        public string appointment_time { get; set; }

        public string message { get; set; }

    }
}
