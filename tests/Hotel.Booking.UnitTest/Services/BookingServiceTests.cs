using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Profiles;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class BookingServiceTests 
    {
       private HotelDbContext dbContext;
        private BookingService service;

        public BookingServiceTests()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
              .UseInMemoryDatabase(nameof(BookingServiceTests))
              .Options;
            dbContext = new HotelDbContext(options);
            CreateRooms(dbContext);
            var respository = new BookingRespository(dbContext);
            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new BookingProfile(),
                new RoomProfile()
            }));

            var mapper = new Mapper(configuration);
            service = new BookingService(respository, mapper);
        }
               

        [Fact]
        public async Task MustReturnAllReservations_GetAllAsync()
        {
            //act
            var result = await service.GetAllAsync();

            //assert
            result.IsSucess.ShouldBeTrue();
            result.Bookings.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAnEmptyListOfReservations_GetAllAsync()
        {
            var serviceMock = new Mock<IBookingService>();
            serviceMock.Setup(_ => _.GetAllAsync()).Returns(GetAllAsyncMock);

            //act
            var result = await serviceMock.Object.GetAllAsync();

            //assert
            result.IsSucess.ShouldBeFalse();
            result.Bookings.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneBookingById_GetByIdAsync()
        {
            //arrange
            var id = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786");

            //act
            var result = await service.GetByIdAsync(id);

            //assert
            result.IsSucess.ShouldBeTrue();
            result.Booking.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task ShoulNotFindTheReservationById_GetByIdAsync()
        {
            //arrange
            var id = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcc");

            //act
            var result = await service.GetByIdAsync(id);

            //assert
            result.IsSucess.ShouldBeFalse();
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustBookRoom_BookRoomAsync()
        {
            //arrange
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(16),
                CheckOut = DateTime.Now.AddDays(19),
                RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd"),
                GuestName = "Elon Musk"
            };

            //act
            var result = await service.BookRoomAsync(bookingRequest);

            //assert
            result.IsSucess.ShouldBeTrue();
            result.Booking.ShouldNotBeNull();
            result.Booking.Id.ToString().ShouldNotBeNull();
            result.Message.ShouldBeEmpty();

        }             

        [Fact]
        public async Task MustUpdateBook_UpdateAsync()
        {
            //arrange
            var expected = new UpdateBookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(16),
                CheckOut = DateTime.Now.AddDays(19),
                BookingId = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786")
            };

            //act
            var result = await service.UpdateAsync(expected);

            //assert
            result.IsSucess.ShouldBeTrue();
            result.Booking.ShouldNotBeNull();
            result.Booking.Id.ToString().ShouldBe(expected.BookingId.ToString());
            result.Booking.CheckIn.Date.ShouldBe(expected.CheckIn.Date);
            result.Booking.CheckOut.Date.ShouldBe(expected.CheckOut.Date);
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustTryUpdateToADateWithReservation_UpdateAsync()
        {
            //arrange
            var request = new UpdateBookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(10),
                CheckOut = DateTime.Now.AddDays(12),
                BookingId = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786")
            };

            //act
            var result = await service.UpdateAsync(request);

            //assert
            result.IsSucess.ShouldBeFalse();
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date.");
        }

        [Fact]
        public async Task MustTryUpdatingANonExistingReservation_UpdateAsync()
        {
            //arrange
            var request = new UpdateBookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(10),
                CheckOut = DateTime.Now.AddDays(12),
                BookingId = Guid.NewGuid()
            };

            //act
            var result = await service.UpdateAsync(request);

            //assert
            result.IsSucess.ShouldBeFalse();
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustCancelBook_CancelAsync()
        {
            //arrange
            var bookingId = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786");

            //act
            var result = await service.CancelAsync(bookingId);

            //assert
            result.IsSucess.ShouldBeTrue();
            result.Booking.ShouldNotBeNull();
            result.Booking.Status.ShouldBe("BookingCanceled");
            result.Message.ShouldBe("Booking successfully canceled");
        }


        [Fact]
        public async Task MustTryCancelANonExistingReservation_CancelAsync()
        {
            //arrange
            var bookingId = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28780");

            //act
            var result = await service.CancelAsync(bookingId);

            //assert
            result.IsSucess.ShouldBeFalse();
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustValidateARoomWithoutReservation_CheckAvailabilityAsync()
        {
            //arrange
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(5),
                CheckOut = DateTime.Now.AddDays(7),
                RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd")
            };

            //act
            var result = await service.CheckAvailabilityAsync(bookingRequest);

            //assert
            result.IsSucess.ShouldBeTrue();
            result.Status.ShouldBe(RoomStatusValueObject.Available);
            result.Message.ShouldBe("Room available to book");
        }

        [Fact]
        public async Task MustValidateARoomWithReservation_CheckAvailabilityAsync()
        {
            //arrange         
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(10),
                CheckOut = DateTime.Now.AddDays(12),
                RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd")
            };

            //act
            var result = await service.CheckAvailabilityAsync(bookingRequest);

            //assert
            result.IsSucess.ShouldBeFalse();
            result.Status.ShouldBe(RoomStatusValueObject.Booked);
            result.Message.ShouldBe("Room not available for booking on this date");
        }      


        private async Task<(bool IsSucess, List<BookingResponse> Bookings, string Message)> GetAllAsyncMock() =>
            (false, new List<BookingResponse>(), "Not found");

        private void CreateRooms(HotelDbContext dbContext)
        {
            if (!dbContext.Bookings.Any())
            {
                var roomA = new RoomEntity("MIO Cancún Hotel Boutique, Queen Suite") { Id = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd") };
                dbContext.Rooms.Add(roomA);
                dbContext.Bookings.Add(new BookingEntity(DateTime.Now.AddDays(1), DateTime.Now.AddDays(4), roomA.Id, "Aegon Targaryen") { Id = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786") });
                dbContext.Bookings.Add(new BookingEntity(DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomA.Id, "Jhow Snow") { Id = Guid.Parse("28630ed2-55a0-4b7e-bfa2-31a68502296c") });

                var roomB = new RoomEntity("Hotel Krystal Cancún, Standard King Room")
                {
                    Id = Guid.NewGuid(),
                    IsActive = false
                };
                dbContext.Rooms.Add(roomB);
                dbContext.Bookings.Add(new BookingEntity(DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), roomB.Id, "Daenys Targaryen") { Id = Guid.Parse("3de2f62e-fe55-4395-a31f-6c4a492ed1cb") });
                dbContext.Bookings.Add(new BookingEntity(DateTime.Now.AddDays(11), DateTime.Now.AddDays(14), roomB.Id, "Arya Stark") { Id = Guid.Parse("347d12d1-7c76-4264-9d0c-25139be60ddb") });

                dbContext.SaveChanges();
            }
        }
    }
}