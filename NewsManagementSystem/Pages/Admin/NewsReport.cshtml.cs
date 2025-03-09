using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NewsManagementSystem.Pages.Admin
{
    public class NewsReportModel : PageModel
    {
        private readonly NewsService _newsService;

        public NewsReportModel(NewsService newsService)
        {
            _newsService = newsService;
        }

        public IEnumerable<NewsArticle> NewsList { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public async Task OnGetAsync()
        {
            NewsList = await _newsService.GetNewsByDateRange(StartDate, EndDate);
        }
    }
}
