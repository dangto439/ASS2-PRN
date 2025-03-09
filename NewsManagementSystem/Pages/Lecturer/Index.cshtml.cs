using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;

namespace NewsManagementSystem.Pages.News
{
    public class IndexModel : PageModel
    {
        private readonly NewsService _newsService;

        public IndexModel(NewsService newsService)
        {
            _newsService = newsService;
        }

        public IEnumerable<NewsArticle> NewsList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTitle { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(SearchTitle))
            {
                NewsList = await _newsService.GetNewsByTitle(SearchTitle);
            }
            else
            {
                NewsList = await _newsService.GetAllActiveNews();
            }
            return Page();
        }
    }
}
