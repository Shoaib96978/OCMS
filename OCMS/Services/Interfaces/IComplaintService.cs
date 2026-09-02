using OCMS.DTOs.Complaint;
using OCMS.Shared;

namespace OCMS.Services.Interfaces
{
    public interface IComplaintService
    {
        Task<AppResponse> SubmitAsync(AddComplaintDto dto, Guid userId);
        Task<AppResponse> GetAllAsync();
        Task<AppResponse> GetByUserIdAsync(Guid userId);
        Task<AppResponse> GetByTrackIdAsync(string trackId);
        Task<AppResponse> GetByIdAsync(Guid complaintId);
        Task<AppResponse> UpdateStatusAsync(Guid complaintId, int status);
        Task<AppResponse> DeleteAsync(Guid complaintId);
    }
}