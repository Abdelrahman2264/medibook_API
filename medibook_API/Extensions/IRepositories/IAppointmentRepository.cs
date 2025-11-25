using medibook_API.Extensions.DTOs;

namespace medibook_API.Extensions.IRepositories
{
    public interface IAppointmentRepository
    {
        public Task<AppointmentResponseDto> CreateAppintmentAsync(CreateAppoinmentDto dto);
        public Task<bool> AssignAppointmentAsync(AssignAppoinmentDto dto);
        public Task<bool> CloseAppointmentAsync(CloseAppointmentDto dto);
        public Task<bool> IfAppointmentDateNotAvailableAsync(DateTime time);
        public Task<AppointmentResponseDto> CancelAppointmentAsync(CancelAppointmentDto dto);

        public Task<AppointmentDetailsDto> GetAppointmentByAppoitnmentIdAsync(int id);
        public Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentAsync();
        public Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentByPatientIdAsync(int id);
        public Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentByDoctorIdAsync(int id);
        public Task<IEnumerable<AppointmentDetailsDto>> GetAllAppointmentByNurseIdAsync(int id);
        public Task<IEnumerable<DateTime>> GetAllActiveAppointmentDatesAsync();


    }
}
