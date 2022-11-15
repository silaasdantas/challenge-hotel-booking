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
        
    }
}
