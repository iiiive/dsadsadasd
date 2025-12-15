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

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public async Task<T> AddAsync(T entity) { _context.Set<T>().Add(entity); await _context.SaveChangesAsync(); return entity; }
        public async Task<T?> UpdateAsync(T entity) { _context.Set<T>().Update(entity); await _context.SaveChangesAsync(); return entity; }
        public async Task<bool> DeleteAsync(int id) { var e = await _context.Set<T>().FindAsync(id); if (e == null) return false; _context.Set<T>().Remove(e); await _context.SaveChangesAsync(); return true; }
        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);
    }
}
