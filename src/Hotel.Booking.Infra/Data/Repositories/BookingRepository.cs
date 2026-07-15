using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Specifications;
using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data.Repositories
{
    public class BookingRepository : EfRepository<BookingEntity>, IBookingRepository
    {
        private static readonly SemaphoreSlim BookingWriteLock = new(1, 1);

        public BookingRepository(HotelDbContext dbContext)
            : base(dbContext) { }

        public async Task<List<BookingEntity>> GetAllAsync(CancellationToken cancellationToken) =>
         await _dbSet.AsNoTracking().Include(_ => _.Room).ToListAsync(cancellationToken);

        public async Task<RoomStatusValueObject> CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut, Guid? ignoredBookingId, CancellationToken cancellationToken)
        {
            var result = await _dbSet.AsQueryable()
                                     .AsNoTracking()
                                     .Include(_ => _.Room)
                                     .AnyAsync(BookingAvailabilitySpecification.ConflictsWith(roomId, checkIn, checkOut, ignoredBookingId), cancellationToken);

            if (result)
                return RoomStatusValueObject.Booked;

            return RoomStatusValueObject.Available;
        }

        public async Task<(bool IsSuccess, BookingEntity Booking)> TryCreateBookingAsync(BookingEntity booking, CancellationToken cancellationToken)
        {
            await BookingWriteLock.WaitAsync(cancellationToken);
            try
            {
                var status = await CheckRoomAvailabilityAsync(booking.RoomId, booking.CheckIn, booking.CheckOut, null, cancellationToken);
                if (!status.Equals(RoomStatusValueObject.Available))
                    return (false, booking);

                _dbSet.Add(booking);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return (true, booking);
            }
            finally
            {
                BookingWriteLock.Release();
            }
        }

        public async Task<(bool IsSuccess, bool NotFound, BookingEntity? Booking)> TryUpdateBookingDatesAsync(Guid bookingId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
        {
            await BookingWriteLock.WaitAsync(cancellationToken);
            try
            {
                var booking = await _dbSet.Include(_ => _.Room).FirstOrDefaultAsync(_ => _.Id.Equals(bookingId), cancellationToken);
                if (booking == null)
                    return (false, true, null);

                var status = await CheckRoomAvailabilityAsync(booking.RoomId, checkIn, checkOut, booking.Id, cancellationToken);
                if (!status.Equals(RoomStatusValueObject.Available))
                    return (false, false, null);

                booking.Update(checkIn, checkOut);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return (true, false, booking);
            }
            finally
            {
                BookingWriteLock.Release();
            }
        }
    }
}
