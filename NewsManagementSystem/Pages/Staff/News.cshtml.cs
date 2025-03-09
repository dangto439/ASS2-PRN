using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsManagementSystem.Pages.Staff
{
    public class NewsModel : PageModel
    {
        private readonly NewsService _newsService;
        private readonly CategoryService _categoryService;
        private readonly AccountService _accountService;
        private readonly TagService _tagService;

        public NewsModel(
            NewsService newsService,
            CategoryService categoryService,
            AccountService accountService,
            TagService tagService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _accountService = accountService;
            _tagService = tagService;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SystemAccount> Accounts { get; set; }
        public IEnumerable<Tag> Tags { get; set; }

        [BindProperty(SupportsGet = true)]
        public short AccountId { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Authentication/Index");
            }

            AccountId = (short)accountId;
            ViewData["AccountId"] = AccountId;
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
                newsArticle.CreatedById = (short)HttpContext.Session.GetInt32("AccountId");
                await _newsService.AddNews(newsArticle);

                // Thêm Tag vào bài viết
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

        public async Task<IActionResult> OnPostEditAsync([FromForm] NewsArticle newsArticle, [FromForm] int TagId)
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
                existingArticle.Headline = newsArticle.Headline;
                existingArticle.CreatedDate = newsArticle.CreatedDate;
                existingArticle.NewsContent = newsArticle.NewsContent;
                existingArticle.NewsSource = newsArticle.NewsSource;
                existingArticle.CategoryId = newsArticle.CategoryId;
                existingArticle.NewsStatus = newsArticle.NewsStatus;
                existingArticle.UpdatedById =  (short)HttpContext.Session.GetInt32("AccountId");
                existingArticle.ModifiedDate = DateTime.Now;

                await _newsService.UpdateNews(existingArticle);

              
                var currentTagId = existingArticle.Tags?.FirstOrDefault()?.TagId;
                if (currentTagId.HasValue)
                {
                    await _tagService.RemoveTagsFromNews(existingArticle.NewsArticleId, new List<int> { currentTagId.Value });
                }
                if (TagId > 0)
                {
                    await _tagService.AddTagsToNews(existingArticle.NewsArticleId, new List<int> { TagId });
                }

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
            Tags = await _tagService.GetAllTags();
        }
    }
}