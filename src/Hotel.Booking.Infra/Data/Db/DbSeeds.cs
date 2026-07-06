using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Infra.Data.Db
{
    public static class DbSeeds
    {
        public static void SeedData(HotelDbContext dbContext)
        {
            //if (dbContext.Bookings.Any())
            //    dbContext.Bookings.RemoveRange(Bookings.ToList());

            //if (Rooms.Any())
            //    Rooms.RemoveRange(Rooms.ToList());

            if (!dbContext.Rooms.Any())
            {
                var roomA = new RoomEntity(Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd"), "MIO Cancún Hotel Boutique, Queen Suite");
                dbContext.Rooms.Add(roomA);
                dbContext.Bookings.Add(new BookingEntity(Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786"), DateTime.Now.AddDays(1), DateTime.Now.AddDays(4), roomA.Id, "Aegon Targaryen"));
                dbContext.Bookings.Add(new BookingEntity(Guid.Parse("28630ed2-55a0-4b7e-bfa2-31a68502296c"), DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomA.Id, "Jhow Snow"));
                var roomB = new RoomEntity(Guid.NewGuid(), "Hotel Krystal Cancún, Standard King Room");
                roomB.Deactivate();
                dbContext.Rooms.AddRange(roomA, roomB);

                dbContext.SaveChanges();
            }
        }
    }
}
