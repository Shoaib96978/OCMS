using OCMS.Entities;
using OCMS.Repositories;
using OCMS.Services.Interfaces;
using OCMS.Shared;
using OCMS.DTOs.Category;

namespace OCMS.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepo;

        public CategoryService(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<AppResponse> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllWithIncludeAsync(c => c.Complaints);

            var result = categories.Select(c => new GetCategoryDto(
                CategoryId: c.CategoryId,
                CategoryName: c.CategoryName,
                ComplaintCount: c.Complaints.Count
            ));

            return AppResponse.Ok("Categories fetched.", data: result);
        }

        public async Task<AppResponse> AddAsync(string categoryName)
        {
            var exists = await _categoryRepo.ExistsAsync(
                c => c.CategoryName.ToLower() == categoryName.ToLower());

            if (exists)
                return AppResponse.Fail("This category already exists.");

            await _categoryRepo.AddAsync(new Category { CategoryName = categoryName });

            return AppResponse.Ok("Category added successfully.");
        }

        public async Task<AppResponse> UpdateAsync(int categoryId, string categoryName)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryId);

            if (category == null)
                return AppResponse.Fail("Category not found.");

            category.CategoryName = categoryName;
            await _categoryRepo.UpdateAsync(category);

            return AppResponse.Ok("Category updated successfully.");
        }

        public async Task<AppResponse> DeleteAsync(int categoryId)
        {
            var category = await _categoryRepo.GetByIdWithIncludeAsync(
                categoryId, c => c.Complaints);

            if (category == null)
                return AppResponse.Fail("Category not found.");

            if (category.Complaints.Any())
                return AppResponse.Fail("Cannot delete — complaints exist in this category.");

            await _categoryRepo.DeleteByEntityAsync(category);

            return AppResponse.Ok("Category deleted successfully.");
        }
    }
}