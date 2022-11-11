using Hotel.Booking.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options)
            : base(options) { }
        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<BookingEntity> Bookings { get; set; }

    }
}
