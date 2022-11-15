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
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(1), DateTime.Now.AddDays(4), roomA.Id) { Id = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786") });
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomA.Id) { Id = Guid.Parse("28630ed2-55a0-4b7e-bfa2-31a68502296c") });

                var roomB = new RoomEntity("Hotel Krystal Cancún, Standard King Room")
                {
                    Id = Guid.Parse("d91ab54e-e4e5-43a3-a28d-6521372489dd"),
                    IsActive = false
                };
                Rooms.Add(roomB);
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), roomB.Id) { Id = Guid.Parse("3de2f62e-fe55-4395-a31f-6c4a492ed1cb") });
                Bookings.Add(new BookingEntity(DateTime.Now.AddDays(11), DateTime.Now.AddDays(14), roomB.Id) { Id = Guid.Parse("347d12d1-7c76-4264-9d0c-25139be60ddb") });

                SaveChanges();
            }
        }
    }
}
