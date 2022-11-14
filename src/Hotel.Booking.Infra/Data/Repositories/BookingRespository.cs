using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data.Repositories
{
    public class BookingRespository : EfRepository<BookingEntity>, IBookingRespository
    {
        public BookingRespository(HotelDbContext dbContext)
            : base(dbContext) { }

        public async Task<List<BookingEntity>> GetAllAsync() =>
         await _dbSet.AsNoTracking().Include(_ => _.Room).ToListAsync();

        public async Task<RoomStatusValueObject> CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var result = await _dbSet.AsQueryable().AsNoTracking()
                               .AnyAsync(_ => _.RoomId.Equals(roomId)
                                           && _.CheckIn.Date <= checkIn.Date && _.CheckOut.Date >= checkIn.Date
                                           || _.CheckIn.Date <= checkOut.Date && _.CheckOut.Date >= checkIn.Date);
            if (result)
                return RoomStatusValueObject.Booked;
            else
                return RoomStatusValueObject.Available;
        } 
    }
}
