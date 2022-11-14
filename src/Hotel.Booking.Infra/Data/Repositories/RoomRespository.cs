using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data.Repositories
{
    public class RoomRespository : EfRepository<RoomEntity>, IRoomRespository
    {
        public RoomRespository(HotelDbContext dbContext)
            : base(dbContext) { }

        public async Task<List<RoomEntity>> GetAllAsync() => 
            await _dbSet.AsNoTracking().Where(_ => _.IsActive).ToListAsync();

        public async Task<RoomStatusValueObject> CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var result = await _dbSet.AsQueryable().AsNoTracking()
                               .Include(_ => _.Bookings)
                               .AnyAsync(_ => _.Id.Equals(roomId) && _.IsActive && _.Bookings.Any(booking =>
                               (booking.CheckIn.Date <= checkIn.Date && booking.CheckOut.Date >= checkIn.Date) ||
                               (booking.CheckIn.Date <= checkOut.Date && booking.CheckOut.Date >= checkIn.Date)));

            if (result)
                return RoomStatusValueObject.Booked;
            else
                return RoomStatusValueObject.Available;
        }
    }
}
