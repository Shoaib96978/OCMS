using OCMS.DTOs.Complaint;
using OCMS.Entities;
using OCMS.Mappers.Complaint;
using OCMS.Repositories;
using OCMS.Services.Interfaces;
using OCMS.Shared;
using OCMS.Shared.Enums;
using OCMS.Shared.Helpers;
using System.Linq.Expressions;

namespace OCMS.Services.Implementations
{
    public class ComplaintService : IComplaintService
    {
        private readonly IRepository<Complaint> _complaintRepo;

        public ComplaintService(IRepository<Complaint> complaintRepo)
        {
            _complaintRepo = complaintRepo;
        }

        // ===================== SUBMIT =====================
        public async Task<AppResponse> SubmitAsync(AddComplaintDto dto, Guid userId)
        {
            var imagePath = await ImageService.SaveAsync(dto.ImageFile);

            var complaint = dto.MapToAddComplaint(userId, imagePath);

            await _complaintRepo.AddAsync(complaint);
            return await _complaintRepo.SaveChangesAsync() > 0 ?
             AppResponse.Ok(
                 message: $"Complaint submitted successfully! Your Track ID is {complaint.TrackId}",
                 data: new { complaint.TrackId }
             )
             : AppResponse.Fail("Internal Server Error");
        }

        // ===================== GET ALL =====================
        public async Task<AppResponse> GetAllAsync()
        {
            var complaints = await _complaintRepo.GetAllWithIncludeAsync(
                c => c.User,
                c => c.Category
            );

            var result = complaints.MapToGetComplaintDtoList();

            return AppResponse.Ok("Complaints fetched successfully.", data: result);
        }
        public async Task<AppResponse> GetFilteredAsync(ComplaintFilterDto dto)
        {
            Expression<Func<Complaint, bool>> filter = c =>
                (dto.Status == null || (int)c.Status == dto.Status) &&
                (string.IsNullOrEmpty(dto.CategoryName) || c.Category!.CategoryName == dto.CategoryName) &&
                (dto.DateFrom == null || c.SubmissionDate >= dto.DateFrom) &&
                (string.IsNullOrEmpty(dto.Search) ||
                    c.Title.Contains(dto.Search) ||
                    c.TrackId.Contains(dto.Search) ||
                    c.User!.FullName.Contains(dto.Search));

            var totalCount = await _complaintRepo.CountAsync(filter);

            var complaints = await _complaintRepo.GetPagedWithIncludeAsync(
                dto.Page,
                dto.PageSize,
                filter,
                orderBy: c => c.SubmissionDate,
                isDescending: true,
                includes: [c => c.Category!, c => c.User!]
            );

            var mapped = complaints.Select(c => c.MapToGetComplaintDto());

            return AppResponse.Ok("Complaints fetched.", data: new
            {
                items = mapped,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize),
                page = dto.Page
            });
        }
        // ===================== GET BY USER =====================
        public async Task<AppResponse> GetByUserIdAsync(Guid userId)
        {
            var complaints = await _complaintRepo.GetWhereWithIncludeAsync(
                filter: c => c.UserId == userId,
                c => c.Category
            );

            var result = complaints.MapToGetComplaintDtoList();

            return AppResponse.Ok("User complaints fetched.", data: result);
        }

        // ===================== GET BY TRACK ID =====================
        public async Task<AppResponse> GetByTrackIdAsync(string trackId)
        {
            var complaint = await _complaintRepo.GetFirstOrDefaultWithIncludeAsync(x => x.TrackId == trackId,
                c => c.User,
                c => c.Category,
                c => c.Responses
            );

            if (complaint == null)
                return AppResponse.Fail("No complaint found with this Track ID.");

            return AppResponse.Ok("Complaint found.", data: complaint.MapToGetComplaintDto());
        }

        // ===================== GET BY ID =====================
        public async Task<AppResponse> GetByIdAsync(Guid complaintId)
        {
            var complaint = await _complaintRepo.GetByIdWithIncludeAsync(
                complaintId,
                c => c.User,
                c => c.Category,
                c => c.Responses
            );

            if (complaint == null)
                return AppResponse.Fail("Complaint not found.");

            return AppResponse.Ok("Complaint fetched.", data: complaint.MapToGetComplaintDto());
        }

        // ===================== UPDATE STATUS =====================
        public async Task<AppResponse> UpdateStatusAsync(Guid complaintId, int status)
        {
            var complaint = await _complaintRepo.GetByIdAsync(complaintId);

            if (complaint == null)
                return AppResponse.Fail("Complaint not found.");

            complaint.Status = (ComplaintStatus)status;

            await _complaintRepo.UpdateAsync(complaint);
            var result = await _complaintRepo.SaveChangesAsync();
            return result > 0 ? AppResponse.Ok("Complaint status updated successfully.") : AppResponse.Fail("Server Error !");
        }

        // ===================== DELETE =====================
        public async Task<AppResponse> DeleteAsync(Guid complaintId)
        {
            var complaint = await _complaintRepo.GetByIdAsync(complaintId);

            if (complaint == null)
                return AppResponse.Fail("Complaint not found.");

            ImageService.Delete(complaint.ImagePath);

            await _complaintRepo.DeleteByEntityAsync(complaint);
            var result = await _complaintRepo.SaveChangesAsync();

            return result > 0 ? AppResponse.Ok("Complaint deleted successfully.") : AppResponse.Fail("Server Error !");
        }
    }
}