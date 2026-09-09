namespace OCMS.DTOs.Complaint;

public class ComplaintFilterDto
{
    public int? Status { get; set; }
    public string? CategoryName { get; set; }
    public DateTime? DateFrom { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}