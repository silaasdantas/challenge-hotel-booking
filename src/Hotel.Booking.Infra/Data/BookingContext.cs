using Hotel.Booking.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options)
            : base(options) { SeedData(); }

        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<BookingEntity> Bookings { get; set; }

        private void SeedData()
        {
            if (!Rooms.Any())
            {
                var roomSeed = new RoomEntity("Premium Suite, Lagoon View");
                Rooms.Add(roomSeed);
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), roomSeed.Id));
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomSeed.Id));
                SaveChanges();
            }
        }
    }
}
