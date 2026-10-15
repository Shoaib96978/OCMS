namespace OCMS.DTOs.User;

public class UserFilterDto
{
    public int? Status { get; set; }   // 0=Active, 1=Inactive, 2=Blocked
    public int? Role { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}