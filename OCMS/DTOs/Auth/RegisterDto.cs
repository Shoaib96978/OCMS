namespace OCMS.DTOs.Auth;
public record RegisterDto(string FullName, string Email, string Password, string ConfirmPassword);
