using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsManagementSystem.Pages.Admin
{
    public class NewsReportModel : PageModel
    {
        private readonly NewsService _newsService;
        private readonly CategoryService _categoryService;
        private readonly TagService _tagService; // Thêm TagService

        public NewsReportModel(
            NewsService newsService,
            CategoryService categoryService,
            TagService tagService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Tag> Tags { get; set; } // Thêm danh sách Tag

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] NewsArticle newsArticle, [FromForm] int TagId)
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ErrorMessage = "Please correct the errors and try again.";
                return Page();
            }

            try
            {
                newsArticle.CreatedDate = DateTime.Now;
             
                await _newsService.AddNews(newsArticle);

                
                if (TagId > 0)
                {
                    await _tagService.AddTagsToNews(newsArticle.NewsArticleId, new List<int> { TagId });
                }

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

        private async Task LoadDataAsync()
        {
            var allNews = await _newsService.GetAllNews();

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                allNews = allNews.Where(n =>
                    (n.NewsTitle != null && n.NewsTitle.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    (n.Headline != null && n.Headline.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    (n.NewsContent != null && n.NewsContent.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (StartDate.HasValue)
            {
                allNews = allNews.Where(n => n.CreatedDate.HasValue && n.CreatedDate.Value.Date >= StartDate.Value.Date);
            }

            if (EndDate.HasValue)
            {
                allNews = allNews.Where(n => n.CreatedDate.HasValue && n.CreatedDate.Value.Date <= EndDate.Value.Date);
            }

            NewsArticles = allNews;
            Categories = await _categoryService.GetAllCategories();
            Tags = await _tagService.GetAllTags(); 
        }
    }
}