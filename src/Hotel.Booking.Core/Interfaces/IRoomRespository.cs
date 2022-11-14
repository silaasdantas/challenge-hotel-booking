using Hotel.Booking.Core.Entities;
using System.Linq.Expressions;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IBookingRespository
    {
        Task<RoomStatusValueObject> CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
        Task<List<BookingEntity>> GetAllAsync();
        Task<BookingEntity> GetByIdAsync(Guid id);
        Task<bool> AnyAsync(Expression<Func<BookingEntity, bool>> predicate);
        Task<int> CreateAsync(BookingEntity entity);
        Task<int> UpdateAsync(BookingEntity entity);
    }
}
