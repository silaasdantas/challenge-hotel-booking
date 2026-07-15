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

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken) 
            => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        public async Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate, cancellationToken);
        }

        public IQueryable<T> GetByQueryable(params Expression<Func<T, object>>[] expressions)
        {
            var query = _dbSet.AsQueryable().AsNoTracking();
            foreach (var expression in expressions)
                query = query.Include(expression);
            return query;
        }

        public Task<int> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Add(entity);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<int> UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
            _dbSet.Update(entity);
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
