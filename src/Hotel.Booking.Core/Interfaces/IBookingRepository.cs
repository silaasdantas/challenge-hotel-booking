using Hotel.Booking.Core.Entities;
using System.Linq.Expressions;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IBookingRepository
    {
        Task<RoomStatusValueObject> CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut, Guid? ignoredBookingId, CancellationToken cancellationToken);
        Task<(bool IsSuccess, BookingEntity Booking)> TryCreateBookingAsync(BookingEntity booking, CancellationToken cancellationToken);
        Task<(bool IsSuccess, bool NotFound, BookingEntity? Booking)> TryUpdateBookingDatesAsync(Guid bookingId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken);
        Task<List<BookingEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<BookingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> AnyAsync(Expression<Func<BookingEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<int> CreateAsync(BookingEntity entity, CancellationToken cancellationToken);
        Task<int> UpdateAsync(BookingEntity entity, CancellationToken cancellationToken);
    }
}
