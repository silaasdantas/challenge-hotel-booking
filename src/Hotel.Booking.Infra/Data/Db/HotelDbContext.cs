using Hotel.Booking.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Infra.Data.Db
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options) { SeedData(); }

        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<BookingEntity> Bookings { get; set; }

        private void SeedData()
        {
            if (!Rooms.Any())
            {
                var roomA = new RoomEntity("MIO Cancún Hotel Boutique, Queen Suite") { Id = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd") };
                Rooms.Add(roomA);
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), roomA.Id));
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomA.Id));

                var roomB = new RoomEntity("Hotel Krystal Cancún, Standard King Room")
                {
                    Id = Guid.Parse("d91ab54e-e4e5-43a3-a28d-6521372489dd"),
                    IsActive = false
                };
                Rooms.Add(roomB);
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), roomB.Id));
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(11), DateTime.Now.AddDays(14), roomB.Id));

                SaveChanges();
            }
        }
    }
}
