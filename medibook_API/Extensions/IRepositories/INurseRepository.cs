using medibook_API.Extensions.DTOs;

namespace medibook_API.Extensions.IRepositories
{
    public interface INurseRepository
    {
        public Task<CreatedResponseDto> CreateNurseAsync(CreateNurseDto dto);
        public Task<NurseDetailsDto> UpdateNurseAsync(int id, UpdateNurseDto dto);
        public Task<NurseDetailsDto> GetNurseByIdAsync(int id);
        public Task<IEnumerable<NurseDetailsDto>> GetAllNursesAsync();
        public Task<IEnumerable<NurseDetailsDto>> GetAllActiveNursesAsync();

    }
}
