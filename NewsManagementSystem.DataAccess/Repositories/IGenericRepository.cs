using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewsManagementSystem.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetByDelegate(Func<T, bool> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> GetById(short id);
        Task<T> GetById(string id);
        Task Add(T entity);
        Task Update(T entity);
        Task<bool> Delete(short id);
        Task<bool> Delete(string id);

    }
}
