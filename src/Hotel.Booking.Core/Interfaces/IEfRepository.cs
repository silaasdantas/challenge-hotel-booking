using System.Linq.Expressions;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IEfRepository<T> : IDisposable where T : class
    {
        Task<T?> GetByIdAsync(Guid id);     
        Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CreateAsync(T entity);
        Task<int> UpdateAsync(T entity);
        IQueryable<T> GetByQueryable(params Expression<Func<T, object>>[] expressions);
    }
}
