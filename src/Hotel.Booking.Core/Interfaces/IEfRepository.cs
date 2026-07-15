using System.Linq.Expressions;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IEfRepository<T>  where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<int> CreateAsync(T entity, CancellationToken cancellationToken);
        Task<int> UpdateAsync(T entity, CancellationToken cancellationToken);
        IQueryable<T> GetByQueryable(params Expression<Func<T, object>>[] expressions);
    }
}
