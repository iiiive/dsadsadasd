using System.Linq.Expressions;

namespace ApungLourdesWebApi.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);

        


        // Add this to support querying by condition
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task SaveAsync();


    }
}
