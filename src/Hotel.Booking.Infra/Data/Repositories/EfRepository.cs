using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hotel.Booking.Infra.Data.Repositories
{
    public class EfRepository<T> : IEfRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet;
        public readonly HotelDbContext _dbContext;

        public EfRepository(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id) 
            => await _dbSet.FindAsync(id);

        public async Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate);
        }

        public IQueryable<T> GetByQueryable(params Expression<Func<T, object>>[] expressions)
        {
            var query = _dbSet.AsQueryable().AsNoTracking();
            foreach (var expression in expressions)
                query = query.Include(expression);
            return query;
        }

        public Task<int> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(T entity)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
            _dbSet.Update(entity);
            return _dbContext.SaveChangesAsync();
        }

    }
}
