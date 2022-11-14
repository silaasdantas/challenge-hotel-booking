using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hotel.Booking.Infra.Data.Repositories
{
    public class BookingRespository : EfRepository<BookingEntity>, IBookingRespository
    {
        public BookingRespository(HotelDbContext dbContext)
            : base(dbContext) { }

        public async Task<BookingEntity> GetByIdAsync(Guid id) =>
            await _dbSet.FindAsync(id);

        public async Task<List<BookingEntity>> GetAllAsync() =>
            await _dbSet.AsNoTracking().Include(_=>_.Room).ToListAsync();

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

        public async Task<bool> AnyAsync(Expression<Func<BookingEntity, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate);
        }

        public Task<int> CreateAsync(BookingEntity entity)
        {
            _dbSet.Add(entity);
            return _dbContext.SaveChangesAsync();
        }
        public Task<int> UpdateAsync(BookingEntity entity)
        {
            //var entity = _dbContext.Bookings.Attach(booking);
            //entity.State = EntityState.Modified;
            //_dbSet.Attach(entity);
            _dbSet.Entry(entity).State = EntityState.Modified;
            _dbSet.Update(entity);
            return _dbContext.SaveChangesAsync();
        }
    }
}
