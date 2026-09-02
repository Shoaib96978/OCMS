namespace OCMS.DTOs.Complaint;

public record AddComplaintDto(string Title, string Description, int CategoryId, IFormFile? ImageFile);
