using OCMS.DTOs.User;
using OCMS.Shared;

namespace OCMS.Services.Interfaces
{
    public interface IUserService
    {
        Task<AppResponse> GetProfileAsync(Guid userId);
        Task<AppResponse> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task<AppResponse> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task<AppResponse> UploadImageAsync(Guid userId, IFormFile imageFile);

        // ======= Admin Use Case =======
        Task<AppResponse> GetAllUsersAsync();
        Task<AppResponse> ToggleStatusAsync(Guid userId);
        Task<AppResponse> DeleteUserAsync(Guid userId);
    }
}
