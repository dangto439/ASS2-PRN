using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NewsManagementSystem.DataAccess.Models;
using NewsManagementSystem.DataAccess.Repositories;
using NewsManagementSystem.Hubs;

namespace NewsManagementSystem.BusinessLogic.Services
{
    public class NewsService
    {
        private readonly IGenericRepository<NewsArticle> _newsRepository;
        private readonly CategoryService _categoryService;
        private readonly AccountService _accountService;
        private readonly IHubContext<NewsHub> _hubContext;
        private readonly FunewsManagementContext _context;

        public NewsService(
            IGenericRepository<NewsArticle> newsRepository,
            CategoryService categoryService,
            AccountService accountService,
            IHubContext<NewsHub> hubContext,
            FunewsManagementContext context)
        {
            _newsRepository = newsRepository;
            _categoryService = categoryService;
            _accountService = accountService;
            _hubContext = hubContext;
            _context = context;
        }

        public async Task<IEnumerable<NewsArticle>> GetAllNews()
        {
            return await _newsRepository.GetAll();
        }

        public async Task<IEnumerable<NewsArticle>> GetAllActiveNews()
        {
            return await _context.NewsArticles
                .Where(n => n.NewsStatus == true)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .ToListAsync();
        }

        public async Task<NewsArticle> GetNewsById(string id)
        {
            return await _newsRepository.GetById(id);
        }

        public async Task AddNews(NewsArticle news)
        {
            await _newsRepository.Add(news);
            await _hubContext.Clients.All.SendAsync("NewsAdded", news);
        }

        public async Task UpdateNews(NewsArticle news)
        {
            await _newsRepository.Update(news);
            await _hubContext.Clients.All.SendAsync("NewsUpdated", news);
        }

        public async Task DeleteNews(string id)
        {
            var news = await _context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (news != null)
            {
                news.Tags.Clear();
                await _newsRepository.Update(news);
                await _newsRepository.Delete(id);
                await _hubContext.Clients.All.SendAsync("NewsDeleted", id);
            }
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCategory(int categoryId)
        {
            return await _context.NewsArticles
                .Where(n => n.CategoryId == categoryId)
                .Include(n => n.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCreator(int creatorId)
        {
            return await _context.NewsArticles
                .Where(n => n.CreatedById == creatorId)
                .Include(n => n.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByDateRange(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles.AsQueryable();

            if (startDate != null && endDate != null)
            {
                query = query.Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate);
            }

            return await query
                .Include(n => n.Tags)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByTitle(string title)
        {
            return await _context.NewsArticles
                .Where(n => n.NewsTitle.Contains(title))
                .Include(n => n.Tags)
                .ToListAsync();
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
