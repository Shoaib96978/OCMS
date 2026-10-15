namespace OCMS.DTOs.Auth;

public record AddUserByAdminDto(
    string FullName,
    string Email,
    string Password,
    int Role,
    string? CurrentAdminPassword
);