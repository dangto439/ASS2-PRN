using NewsManagementSystem.DataAccess.Models;
using NewsManagementSystem.DataAccess.Repositories;

namespace NewsManagementSystem.BusinessLogic.Services
{
    public class TagService
    {
        private readonly IGenericRepository<Tag> _tagRepository;
        private readonly IGenericRepository<NewsArticle> _newsRepository;

        public TagService(
            IGenericRepository<Tag> tagRepository,
            IGenericRepository<NewsArticle> newsRepository)
        {
            _tagRepository = tagRepository;
            _newsRepository = newsRepository;
        }

        public async Task<IEnumerable<Tag>> GetAllTags()
        {
            return await _tagRepository.GetAll();
        }

        public async Task AddTag(Tag tag)
        {
            await _tagRepository.Add(tag);
        }

        public async Task UpdateTag(Tag tag)
        {
            await _tagRepository.Update(tag);
        }

        public async Task DeleteTag(short tagId)
        {
            await _tagRepository.Delete(tagId);
        }

        public async Task AddTagsToNews(string newsId, List<int> tagIds)
        {
            var news = await _newsRepository.GetById(newsId);
            if (news == null) return;

            var tags = (await _tagRepository.GetByDelegate(t => tagIds.Contains(t.TagId))).ToList();

            foreach (var tag in tags)
            {
                if (!news.Tags.Any(t => t.TagId == tag.TagId))
                {
                    news.Tags.Add(tag);
                }
            }

            await _newsRepository.Update(news);
        }

        public async Task RemoveTagsFromNews(string newsId, List<int> tagIds)
        {
            var news = (await _newsRepository.GetByDelegate(n => n.NewsArticleId == newsId, n => n.Tags)).FirstOrDefault();
            if (news == null) return;

            news.Tags = news.Tags.Where(tag => !tagIds.Contains(tag.TagId)).ToList();

            await _newsRepository.Update(news);
        }
    }
}