using medibook_API.Extensions.DTOs;
using medibook_API.Extensions.DTOs.medibook_API.Extensions.DTOs;

namespace medibook_API.Extensions.IRepositories
{
    public interface IFeedBackRepository
    {
        Task<FeedbackResponseDto> CreateFeedbackAsync(CreateFeedbackDto dto);
        Task<FeedbackResponseDto> AddDoctorReplyAsync(DoctorReplyDto dto);
        Task<bool> DeleteFeedbackAsync(int feedbackId);
        Task<bool> ToggleFavouriteAsync(int feedbackId);
        Task<IEnumerable<FeedbackDetailsDto>> GetAllFeedbacksAsync();
        Task<IEnumerable<FeedbackDetailsDto>> GetFeedbacksByDoctorIdAsync(int doctorId);
        Task<IEnumerable<FeedbackDetailsDto>> GetFeedbacksByPatientIdAsync(int patientId);
        Task<IEnumerable<FeedbackDetailsDto>> GetFeedbacksByNurseIdAsync(int nurseId);
        Task<FeedbackDetailsDto?> GetFeedbackByIdAsync(int feedbackId);
        Task<bool> UpdateFeedbackAsync(UpdateFeedbackDto dto);
    }
}
