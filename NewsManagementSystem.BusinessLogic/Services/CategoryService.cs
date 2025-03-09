using NewsManagementSystem.DataAccess.Models;
using NewsManagementSystem.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsManagementSystem.BusinessLogic.Services
{
    public class CategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public CategoryService(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAll();
            return categories;
        }
        public async Task<Category> GetCategoryById(short id)
        {
            var category = await _categoryRepository.GetById(id);
            return category;
        }
        public async Task AddCategory(Category category)
        {
            await _categoryRepository.Add(category);
        }
        public async Task UpdateCategory(Category category)
        {
            await _categoryRepository.Update(category);
        }
        public async Task DeleteCategory(short id)
        {
            await _categoryRepository.Delete(id);
        }

    
        public async Task<IEnumerable<object>> GetCategoryList()
        {
            var categories = await _categoryRepository.GetAll();
            return categories.Select(c => new { c.CategoryId, c.CategoryDesciption }).ToList();
        }
    }
}
