using OCMS.DTOs.Auth;
using OCMS.Shared;

namespace OCMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AppResponse> RegisterAsync(RegisterDto dto);
        Task<AppResponse> LoginAsync(LoginDto dto);
        Task<AppResponse> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<AppResponse> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
