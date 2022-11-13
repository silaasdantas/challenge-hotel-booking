using System.Linq.Expressions;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IEfRepository<T> : IDisposable where T : class
    {
        Task<T?> GetByIdAsync<Tid>(Tid id) where Tid : notnull;
        Task<IEnumerable<T>> GetAllAsync();
        Task<IList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        //IQueryable<T> Get(params Expression<Func<T, object>>[] expressions);
        Task<int> CreateAsync(T entity);
        Task<int> UpdateAsync(T entity);
    }
}
