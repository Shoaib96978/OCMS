using OCMS.DTOs.Admin;
using OCMS.Entities;
using OCMS.Repositories;
using OCMS.Services.Interfaces;
using OCMS.Shared;
using OCMS.Shared.Enums;

namespace OCMS.Services.Implementations
{
    public class AdminService(IRepository<Complaint> complaintRepo, IRepository<User> userRepo) : IAdminService
    {
        private readonly IRepository<Complaint> _complaintRepo = complaintRepo;
        private readonly IRepository<User> _userRepo = userRepo;

        public async Task<AppResponse> GetDashboardStatsAsync()
        {
            var complaints = await _complaintRepo
                .GetAllWithIncludeAsync(c => c.Category);

            var complaintList = complaints.ToList();

            // Category stats
            var categoryStats = complaintList
                .GroupBy(c => c.Category.CategoryName)
                .Select(g => new CategoryStatDto(g.Key, g.Count()));

            // Monthly stats — last 6 months
            var monthlyStats = Enumerable.Range(0, 6)
                .Select(i => DateTime.UtcNow.AddMonths(-i))
                .Select(date => new MonthlyStatDto(
                    Month: date.ToString("MMM"),
                    Count: complaintList.Count(c =>
                        c.SubmissionDate.Month == date.Month &&
                        c.SubmissionDate.Year == date.Year)
                ))
                .Reverse();

            var stats = new DashboardStatsDto(
                TotalUsers: await _userRepo.CountAsync(),
                TotalComplaints: complaintList.Count,
                PendingComplaints: complaintList.Count(c => c.Status == ComplaintStatus.Pending),
                InProgressComplaints: complaintList.Count(c => c.Status == ComplaintStatus.InProgress),
                ResolvedComplaints: complaintList.Count(c => c.Status == ComplaintStatus.Resolved),
                CategoryStats: categoryStats,
                MonthlyStats: monthlyStats
            );

            return AppResponse.Ok("Stats fetched.", data: stats);
        }

    }
}