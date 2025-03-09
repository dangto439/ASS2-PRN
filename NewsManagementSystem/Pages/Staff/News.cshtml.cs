using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsManagementSystem.Pages.Staff
{
    public class NewsModel : PageModel
    {
        private readonly NewsService _newsService;
        private readonly CategoryService _categoryService;
        private readonly AccountService _accountService;

        public NewsModel(NewsService newsService, CategoryService categoryService, AccountService accountService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _accountService = accountService;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SystemAccount> Accounts { get; set; }
       
        [BindProperty]
        public SystemAccount Account { get; set; }

        [TempData]
        public string SuccessMessage { get; set; } 

        [TempData]
        public string ErrorMessage { get; set; } 

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] NewsArticle newsArticle)
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ErrorMessage = "Please correct the errors and try again.";
                return Page();
            }

            try
            {
                newsArticle.ModifiedDate = newsArticle.ModifiedDate ?? DateTime.Now;
                newsArticle.CreatedDate = newsArticle.CreatedDate ?? DateTime.Now; 
                await _newsService.AddNews(newsArticle);
                SuccessMessage = "News article created successfully!";
            }
            catch (Exception ex)
            {
                await LoadDataAsync();
                ErrorMessage = $"Failed to create news article: {ex.Message}";
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync([FromForm] NewsArticle newsArticle)
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ErrorMessage = "Please correct the errors and try again.";
                return Page();
            }

            try
            {
                var existingArticle = await _newsService.GetNewsById(newsArticle.NewsArticleId);
                if (existingArticle == null)
                {
                    ErrorMessage = "News article not found.";
                    return NotFound();
                }

              
                existingArticle.NewsTitle = newsArticle.NewsTitle;
                existingArticle.Headline = newsArticle.Headline ?? existingArticle.Headline;
                existingArticle.CreatedDate = newsArticle.CreatedDate ?? existingArticle.CreatedDate;
                existingArticle.NewsContent = newsArticle.NewsContent;
                existingArticle.NewsSource = newsArticle.NewsSource ?? existingArticle.NewsSource;
                existingArticle.CategoryId = newsArticle.CategoryId ?? existingArticle.CategoryId;
                existingArticle.NewsStatus = newsArticle.NewsStatus ?? existingArticle.NewsStatus;
                existingArticle.UpdatedById = newsArticle.UpdatedById ?? existingArticle.UpdatedById;
                existingArticle.ModifiedDate = newsArticle.ModifiedDate ?? DateTime.Now;

                await _newsService.UpdateNews(existingArticle);
                SuccessMessage = "News article updated successfully!";
            }
            catch (Exception ex)
            {
                await LoadDataAsync();
                ErrorMessage = $"Failed to update news article: {ex.Message}";
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            try
            {
                var article = await _newsService.GetNewsById(id);
                if (article == null)
                {
                    ErrorMessage = "News article not found.";
                    return NotFound();
                }

                await _newsService.DeleteNews(id);
                SuccessMessage = "News article deleted successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete news article: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            NewsArticles = await _newsService.GetAllNews();
            Categories = await _categoryService.GetAllCategories();
            Accounts = await _accountService.GetAllAccont(); 
        }
    }
}