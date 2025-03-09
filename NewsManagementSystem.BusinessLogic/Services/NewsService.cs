using Microsoft.AspNetCore.SignalR;
using NewsManagementSystem.DataAccess.Models;
using NewsManagementSystem.DataAccess.Repositories;
using NewsManagementSystem.Pages.Hubs;

namespace NewsManagementSystem.BusinessLogic.Services
{
    public class NewsService
    {
        private readonly IGenericRepository<NewsArticle> _newsRepository;
        private readonly CategoryService _categoryService;
        private readonly AccountService _accountService;
        private readonly IHubContext<NewsHub> _hubContext;
        public NewsService(IGenericRepository<NewsArticle> newsRepository, CategoryService categoryService,
        AccountService accountService, IHubContext<NewsHub> hubContext)
        {
            _newsRepository = newsRepository;
            _categoryService = categoryService;
            _accountService = accountService;
            _hubContext = hubContext; ;
        }
        public async Task<IEnumerable<NewsArticle>> GetAllNews()
        {
            return await _newsRepository.GetAll();
        }

        public async Task<IEnumerable<NewsArticle>> GetAllActiveNews()
        {
            return await _newsRepository.GetByDelegate(n => n.NewsStatus == true, n => n.Category, n => n.CreatedBy);
        }

        public async Task<NewsArticle> GetNewsById(string id)
        {
            return await _newsRepository.GetById(id);
        }
        public async Task AddNews(NewsArticle news)
        {
            await _newsRepository.Add(news);
            await _hubContext.Clients.All.SendAsync("NewsUpdated", "create", news);
        }
        public async Task UpdateNews(NewsArticle news)
        {
            await _newsRepository.Update(news);
            await _hubContext.Clients.All.SendAsync("NewsUpdated", "update", news);
        }
        public async Task DeleteNews(string id)
        {
            var news = (await _newsRepository.GetByDelegate(n => n.NewsArticleId == id, n => n.Tags)).FirstOrDefault();
            if (news != null)
            {
                news.Tags.Clear();
                await _newsRepository.Update(news);
                await _newsRepository.Delete(id);
                await _hubContext.Clients.All.SendAsync("NewsUpdated", "delete", new { NewsArticleId = id });
            }
        }
        
        public Task<IEnumerable<NewsArticle>> GetNewsByCategory(int categoryId)
        {
            return _newsRepository.GetByDelegate(n => n.CategoryId == categoryId);
        }
        public async Task<IEnumerable<NewsArticle>> GetNewsByCreator(int creatorId)
        {
            return await _newsRepository.GetByDelegate(n => n.CreatedById == creatorId, n => n.Tags);
        }
        public async Task<IEnumerable<NewsArticle>> GetNewsByDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return (await _newsRepository.GetAll()).OrderByDescending(n => n.CreatedDate);
            }
            return (await _newsRepository.GetByDelegate(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)).OrderByDescending(n => n.CreatedDate);
        }
        public async Task<IEnumerable<NewsArticle>> GetNewsByTitle(string title)
        {
            return await _newsRepository.GetByDelegate(n => n.NewsTitle.Contains(title));
        }

        public async Task<IEnumerable<object>> GetCategoryDropdown()
        {
            return await _categoryService.GetCategoryList();
        }

        public async Task<IEnumerable<short>> GetAccountDropdown()
        {
            return await _accountService.GetAccountList();
        }
    }

 
}
