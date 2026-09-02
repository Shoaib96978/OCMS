using OCMS.Shared;

namespace OCMS.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<AppResponse> GetAllAsync();
        Task<AppResponse> AddAsync(string categoryName);
        Task<AppResponse> UpdateAsync(int categoryId, string categoryName);
        Task<AppResponse> DeleteAsync(int categoryId);
    }
}