using OCMS.Shared;

namespace OCMS.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AppResponse> GetDashboardStatsAsync();
    }
}