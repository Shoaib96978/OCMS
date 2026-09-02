using OCMS.Shared.Enums;

namespace OCMS.DTOs.Complaint;

public record GetComplaintDto(Guid ComplaintId, string Title, string Description, string CategoryName, string? ImagePath, ComplaintStatus Status, DateTime SubmissionDate, string TrackId, string SubmittedBy);
