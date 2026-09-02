using Microsoft.AspNetCore.Mvc;
using OCMS.Services.Interfaces;

namespace OCMS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Json(await _categoryService.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] string categoryName)
            => Json(await _categoryService.AddAsync(categoryName));

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] string categoryName)
            => Json(await _categoryService.UpdateAsync(id, categoryName));

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
            => Json(await _categoryService.DeleteAsync(id));
    }
}