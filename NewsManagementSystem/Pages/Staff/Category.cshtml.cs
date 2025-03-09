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
        public string AccountId { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
          
            AccountId = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(AccountId))
            {
              
                return RedirectToPage("/Authentication/Index");
            }

           
            ViewData["AccountId"] = AccountId;

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] Category category)
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ErrorMessage = "Invalid data. Please check your inputs.";
                return Page();
            }

            try
            {
                await _categoryService.AddCategory(category);
                SuccessMessage = "Category created successfully!";
            }
            catch (Exception ex)
            {
                await LoadDataAsync();
                ErrorMessage = $"Failed to create category: {ex.Message}";
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync([FromForm] Category category)
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ErrorMessage = "Invalid data. Please check your inputs.";
                return Page();
            }

            try
            {
                var existingCategory = await _categoryService.GetCategoryById(category.CategoryId);
                if (existingCategory == null)
                {
                    ErrorMessage = "Category not found.";
                    return NotFound();
                }

                existingCategory.CategoryName = category.CategoryName;
                existingCategory.CategoryDesciption = category.CategoryDesciption ?? existingCategory.CategoryDesciption;
                existingCategory.ParentCategoryId = category.ParentCategoryId ?? existingCategory.ParentCategoryId;
                existingCategory.IsActive = category.IsActive ?? existingCategory.IsActive;

                await _categoryService.UpdateCategory(existingCategory);
                SuccessMessage = "Category updated successfully!";
            }
            catch (Exception ex)
            {
                await LoadDataAsync();
                ErrorMessage = $"Failed to update category: {ex.Message}";
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAsync(short id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    ErrorMessage = "Category not found.";
                    return NotFound();
                }

                await _categoryService.DeleteCategory(id);
                SuccessMessage = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete category: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            Categories = await _categoryService.GetAllCategories();
        }
    }
}