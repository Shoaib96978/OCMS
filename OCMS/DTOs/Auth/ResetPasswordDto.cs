namespace OCMS.DTOs.Auth;

public record ResetPasswordDto(Guid UserId, string Otp, string Password, string ConfirmPassword);