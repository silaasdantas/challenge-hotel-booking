using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hotel.Booking.Infra.Data
{
    public class EfRepository<T> : IEfRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet;
        public readonly BookingContext _dbContext;

        public EfRepository(BookingContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync<Tid>(Tid id) where Tid : notnull
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();        
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate);
        }

        public async Task<IList<T>> GetTestAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] expressions)
        {
            if (expressions.Any())
            {
                var query = _dbSet.AsQueryable().AsNoTracking();
                foreach (var expression in expressions)
                    query = query.Include(expression);
                return await query.Where(predicate).ToListAsync();
            }

            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<IList<T>> GetFindAsync(params Expression<Func<T, object>>[] expressions)
        {
            var query = _dbSet.AsQueryable().AsNoTracking();
            foreach (var expression in expressions)
                query = query.Include(expression);
            return await query.ToListAsync();
        }

        public Task<int> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(T entity)
        {
            //var entity = _dbContext.Bookings.Attach(booking);
            //entity.State = EntityState.Modified;
            //_dbSet.Attach(entity);
            _dbSet.Entry(entity).State = EntityState.Modified;
            _dbSet.Update(entity);
            return _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
