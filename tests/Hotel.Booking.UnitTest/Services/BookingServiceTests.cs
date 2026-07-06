using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Exceptions;
using Hotel.Booking.Core.Profiles;
using Hotel.Booking.Core.Results;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class BookingServiceTests 
    {
        private readonly HotelDbContext dbContext;
        private readonly BookingService service;
        private readonly IMapper mapper;

        public BookingServiceTests()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString())
              .Options;
            dbContext = new HotelDbContext(options);
            CreateRooms(dbContext);
            var respository = new BookingRespository(dbContext);
            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new BookingProfile(),
                new RoomProfile()
            }));

            mapper = new Mapper(configuration);
            service = new BookingService(respository, mapper);
        }
               

        [Fact]
        public async Task MustReturnAllReservations_GetAllAsync()
        {
            //act
            var result = await service.GetAllAsync();

            //assert
            result.IsSucess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Bookings.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAnEmptyListOfReservations_GetAllAsync()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var emptyDbContext = new HotelDbContext(options);
            var emptyService = new BookingService(new BookingRespository(emptyDbContext), mapper);

            //act
            var result = await emptyService.GetAllAsync();

            //assert
            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Id.ToString().ShouldNotBeNull();
            result.Message.ShouldBeEmpty();

        }             

        [Fact]
        public async Task MustTryBookRoomWithReservation_BookRoomAsync()
        {
            //arrange
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(10),
                CheckOut = DateTime.Now.AddDays(12),
                RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd"),
                GuestName = "Elon Musk"
            };

            //act
            var result = await service.BookRoomAsync(bookingRequest);

            //assert
            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date");
        }

        [Fact]
        public async Task MustRejectEmptyRoomId_BookRoomAsync()
        {
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Today.AddDays(5),
                CheckOut = DateTime.Today.AddDays(7),
                RoomId = Guid.Empty,
                GuestName = "Elon Musk"
            };

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(bookingRequest));

            exception.Message.ShouldBe("RoomId is required.");
        }

        [Fact]
        public async Task MustRejectEmptyGuestName_BookRoomAsync()
        {
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Today.AddDays(5),
                CheckOut = DateTime.Today.AddDays(7),
                RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd"),
                GuestName = " "
            };

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(bookingRequest));

            exception.Message.ShouldBe("GuestName is required.");
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
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustRejectEmptyBookingId_UpdateAsync()
        {
            var request = new UpdateBookingRequest()
            {
                CheckIn = DateTime.Today.AddDays(10),
                CheckOut = DateTime.Today.AddDays(12),
                BookingId = Guid.Empty
            };

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.UpdateAsync(request));

            exception.Message.ShouldBe("BookingId is required.");
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
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
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
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Status.ShouldBe(RoomStatusValueObject.Booked);
            result.Message.ShouldBe("Room not available for booking on this date");
        }

        [Fact]
        public async Task MustTryValidateANonExistingRoom_CheckAvailabilityAsync()
        {
            //arrange
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Now.AddDays(5),
                CheckOut = DateTime.Now.AddDays(7),
                RoomId = Guid.NewGuid()
            };

            //act
            var result = await service.CheckAvailabilityAsync(bookingRequest);

            //assert
            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Status.ShouldBe(RoomStatusValueObject.None);
            result.Message.ShouldBe("Room not found");
        }

        [Fact]
        public async Task MustRejectEmptyRoomId_CheckAvailabilityAsync()
        {
            var bookingRequest = new BookingRequest()
            {
                CheckIn = DateTime.Today.AddDays(5),
                CheckOut = DateTime.Today.AddDays(7),
                RoomId = Guid.Empty
            };

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.CheckAvailabilityAsync(bookingRequest));

            exception.Message.ShouldBe("RoomId is required.");
        }

        private void CreateRooms(HotelDbContext dbContext)
        {
            if (!dbContext.Bookings.Any())
            {
                var roomA = new RoomEntity(Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd"), "MIO Cancún Hotel Boutique, Queen Suite");
                dbContext.Rooms.Add(roomA);
                dbContext.Bookings.Add(new BookingEntity(Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786"), DateTime.Now.AddDays(1), DateTime.Now.AddDays(4), roomA.Id, "Aegon Targaryen"));
                dbContext.Bookings.Add(new BookingEntity(Guid.Parse("28630ed2-55a0-4b7e-bfa2-31a68502296c"), DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomA.Id, "Jhow Snow"));

                var roomB = new RoomEntity(Guid.NewGuid(), "Hotel Krystal Cancún, Standard King Room");
                roomB.Deactivate();
                dbContext.Rooms.Add(roomB);
                dbContext.Bookings.Add(new BookingEntity(Guid.Parse("3de2f62e-fe55-4395-a31f-6c4a492ed1cb"), DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), roomB.Id, "Daenys Targaryen"));
                dbContext.Bookings.Add(new BookingEntity(Guid.Parse("347d12d1-7c76-4264-9d0c-25139be60ddb"), DateTime.Now.AddDays(11), DateTime.Now.AddDays(14), roomB.Id, "Arya Stark"));

                dbContext.SaveChanges();
            }
        }
    }
}
