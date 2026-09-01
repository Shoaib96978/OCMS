namespace OCMS.DTOs.User;

public record ChangePasswordDto(string CurrentPassword, string NewPassword, string ConfirmPassword);
