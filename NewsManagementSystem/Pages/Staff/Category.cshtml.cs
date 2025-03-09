using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsManagementSystem.Pages.Staff
{
    public class CategoryModel : PageModel
    {
        private readonly CategoryService _categoryService;

        public CategoryModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IEnumerable<Category> Categories { get; set; }
        [BindProperty(SupportsGet = true)]
        public short? AccountId { get; set; }

        public async Task OnGetAsync()
        {
            Categories = await _categoryService.GetAllCategories();
        }

        public async Task<IActionResult> OnPostCreateAsync(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToPage();
            }
            await _categoryService.AddCategory(category);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToPage();
            }
            await _categoryService.UpdateCategory(category);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAsync(short id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
            }
            catch
            {
                TempData["ErrorMessage"] = "Category is being used by news articles.";
            }
            return RedirectToPage();
        }
    }
}
