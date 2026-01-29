using ApungLourdesWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace ApungLourdesWebApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApunglourdesDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApunglourdesDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync(); // keeps your existing behavior
            return entity;
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(); // keeps your existing behavior
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _dbSet.FindAsync(id);
            if (e == null) return false;

            _dbSet.Remove(e);
            await _context.SaveChangesAsync(); // keeps your existing behavior
            return true;
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        // ✅ NEW: for code that expects SaveAsync()
        public async Task SaveAsync()
            => await _context.SaveChangesAsync();
    }
}
