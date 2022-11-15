using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Profiles;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class BookingServiceTests
    {
        private DbContextOptions<HotelDbContext> options;
        private HotelDbContext dbContext;
        private BookingRespository respository;
        private BookingProfile roomProfile;
        private BookingService service;

        public BookingServiceTests()
        {
            options = new DbContextOptionsBuilder<HotelDbContext>()
              .UseInMemoryDatabase("BookingTestingDB")
              .Options;
            dbContext = new HotelDbContext(options);
            respository = new BookingRespository(dbContext);
            roomProfile = new BookingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(roomProfile));
            var mapper = new Mapper(configuration);
            service = new BookingService(respository, mapper);
        }

        [Fact]
        public async Task MustValidateARoomWithoutReservation_CheckAvailabilityAsync()
        {
            //arrange
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(4),
                CheckOut = DateTime.Now.AddDays(7),
                RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd")
            };

            //act
            var expected = await service.CheckAvailabilityAsync(bookingRequest);

            //assert
            expected.IsSucess.ShouldBe(true);
            expected.Status.ShouldBe(RoomStatusValueObject.Available);
            expected.Message.ShouldBe("Room available to book");
        }

        [Fact]
        public async Task MustValidateARoomWithReservation_CheckAvailabilityAsync()
        {
            //arrange         
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(1),
                CheckOut = DateTime.Now.AddDays(4),
                RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd")
            };

            //act
            var expected = await service.CheckAvailabilityAsync(bookingRequest);

            //assert
            expected.IsSucess.ShouldBe(true);
            expected.Status.ShouldBe(RoomStatusValueObject.Available);
            expected.Message.ShouldBe("Room available to book");
        }
    }
}