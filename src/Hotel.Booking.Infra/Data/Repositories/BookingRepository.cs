using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Specifications;
using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data.Repositories
{
    public class BookingRepository : EfRepository<BookingEntity>, IBookingRepository
    {
        public BookingRepository(HotelDbContext dbContext)
            : base(dbContext) { }

        public async Task<List<BookingEntity>> GetAllAsync() =>
         await _dbSet.AsNoTracking().Include(_ => _.Room).ToListAsync();

        public async Task<RoomStatusValueObject> CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut, Guid? ignoredBookingId = null)
        {
            var result = await _dbSet.AsQueryable()
                                     .AsNoTracking()
                                     .Include(_ => _.Room)
                                     .AnyAsync(BookingAvailabilitySpecification.ConflictsWith(roomId, checkIn, checkOut, ignoredBookingId));

            if (result)
                return RoomStatusValueObject.Booked;

            return RoomStatusValueObject.Available;
        }
    }
}
