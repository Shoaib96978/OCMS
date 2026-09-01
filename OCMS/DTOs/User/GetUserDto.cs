using OCMS.Shared.Enums;
namespace OCMS.DTOs.User;

public record GetUserDto(
Guid UserId,
string FullName,
string Email,
AppRoles Role,
UserStatus Status,
string? ImageLink,
DateTime CreatedDate,
DateTime? LastLoginDate,
int TotalComplaints
);
