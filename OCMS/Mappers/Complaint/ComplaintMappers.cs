using OCMS.DTOs.Complaint;
using OCMS.Shared.Enums;
using OCMS.Shared.Helpers;

namespace OCMS.Mappers.Complaint
{
    public static class ComplaintMappers
    {
        // DTO → Entity
        public static Entities.Complaint MapToAddComplaint(
            this AddComplaintDto dto, Guid userId, string? imagePath)
        {
            return new Entities.Complaint
            {
                ComplaintId = Guid.NewGuid(),
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                ImagePath = imagePath,
                Status = ComplaintStatus.Pending,
                SubmissionDate = DateTime.UtcNow,
                TrackId = TrackIdGenerator.Generate()
            };
        }

        // Entity → DTO (record constructor use hoga)
        public static GetComplaintDto MapToGetComplaintDto(this Entities.Complaint complaint)
        {
            return new GetComplaintDto(
                ComplaintId: complaint.ComplaintId,
                Title: complaint.Title,
                Description: complaint.Description,
                CategoryName: complaint.Category?.CategoryName ?? string.Empty,
                ImagePath: complaint.ImagePath,
                Status: complaint.Status,
                SubmissionDate: complaint.SubmissionDate,
                TrackId: complaint.TrackId,
                SubmittedBy: complaint.User?.FullName ?? string.Empty
            );
        }

        // List mapping
        public static IEnumerable<GetComplaintDto> MapToGetComplaintDtoList(
            this IEnumerable<Entities.Complaint> complaints)
                => complaints.Select(c => c.MapToGetComplaintDto());
    }
}