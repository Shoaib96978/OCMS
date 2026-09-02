namespace OCMS.DTOs.Admin
{
    public record DashboardStatsDto(
    int TotalUsers,
    int TotalComplaints,
    int PendingComplaints,
    int InProgressComplaints,
    int ResolvedComplaints,
    IEnumerable<CategoryStatDto> CategoryStats,
    IEnumerable<MonthlyStatDto> MonthlyStats
);
}
