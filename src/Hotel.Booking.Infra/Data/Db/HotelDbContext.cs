using Hotel.Booking.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data.Db
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options) {  }

        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<BookingEntity> Bookings { get; set; }

        
    }
}
