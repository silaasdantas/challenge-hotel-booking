using Hotel.Booking.Core.Entities;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Hotel.Booking.Infra.Data.Repositories.Tests
{
    public class BookingRepositoryTests
    {
        private static readonly Guid RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd");

        [Fact]
        public async Task TryCreateBookingAsync_ShouldBlockDuplicateConcurrentBookings()
        {
            await using var dbContext = CreateDbContext();
            SeedRoom(dbContext);
            var repository = new BookingRepository(dbContext);
            var checkIn = DateTime.Today.AddDays(16);
            var checkOut = DateTime.Today.AddDays(19);

            var first = new BookingEntity(checkIn, checkOut, RoomId, "Ada Lovelace")
            {
                Room = dbContext.Rooms.Single()
            };
            var second = new BookingEntity(checkIn, checkOut, RoomId, "Grace Hopper")
            {
                Room = dbContext.Rooms.Single()
            };

            var results = await Task.WhenAll(
                repository.TryCreateBookingAsync(first, CancellationToken.None),
                repository.TryCreateBookingAsync(second, CancellationToken.None));

            results.Count(_ => _.IsSuccess).ShouldBe(1);
            dbContext.Bookings.Count().ShouldBe(1);
        }

        [Fact]
        public async Task TryUpdateBookingDatesAsync_ShouldBlockConflictingUpdate()
        {
            await using var dbContext = CreateDbContext();
            var room = SeedRoom(dbContext);
            var bookingToUpdate = new BookingEntity(Guid.NewGuid(), DateTime.Today.AddDays(16), DateTime.Today.AddDays(19), RoomId, "Ada Lovelace")
            {
                Room = room
            };
            var existingBooking = new BookingEntity(Guid.NewGuid(), DateTime.Today.AddDays(22), DateTime.Today.AddDays(24), RoomId, "Grace Hopper")
            {
                Room = room
            };
            dbContext.Bookings.AddRange(bookingToUpdate, existingBooking);
            await dbContext.SaveChangesAsync();
            var repository = new BookingRepository(dbContext);

            var result = await repository.TryUpdateBookingDatesAsync(
                bookingToUpdate.Id,
                existingBooking.CheckIn,
                existingBooking.CheckOut,
                CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.NotFound.ShouldBeFalse();
            bookingToUpdate.CheckIn.Date.ShouldBe(DateTime.Today.AddDays(16));
            bookingToUpdate.CheckOut.Date.ShouldBe(DateTime.Today.AddDays(19));
        }

        private static HotelDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new HotelDbContext(options);
        }

        private static RoomEntity SeedRoom(HotelDbContext dbContext)
        {
            var room = new RoomEntity(RoomId, "MIO Cancun Hotel Boutique, Queen Suite");
            dbContext.Rooms.Add(room);
            dbContext.SaveChanges();

            return room;
        }
    }
}
