using medibook_API.Extensions.DTOs;
using medibook_API.Models;

namespace medibook_API.Extensions.IRepositories
{ 
    public interface IDoctorRepository
    {
        public Task<CreatedResponseDto> CreateDoctorAsync(CreateDoctorDto dto);
        public Task<DoctorDetailsDto> UpdateDoctorAsync(int id, UpdateDoctorDto dto);
        public Task<DoctorDetailsDto> GetDoctorByIdAsync(int id);
        public Task<IEnumerable<DoctorDetailsDto>> GetAllDoctorsAsync();
        public Task<IEnumerable<DoctorDetailsDto>> GetAllActiveDoctorsAsync();




    }
}
