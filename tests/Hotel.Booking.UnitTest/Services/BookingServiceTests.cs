using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Exceptions;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Profiles;
using Hotel.Booking.Core.Results;
using Moq;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRespository> respositoryMock;
        private readonly BookingService service;

        private static readonly Guid RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd");
        private static readonly Guid BookingId = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786");

        public BookingServiceTests()
        {
            respositoryMock = new Mock<IBookingRespository>();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile>
            {
                new BookingProfile(),
                new RoomProfile()
            }));

            service = new BookingService(respositoryMock.Object, new Mapper(configuration));
        }

        [Fact]
        public async Task MustReturnAllReservations_GetAllAsync()
        {
            respositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<BookingEntity> { CreateBooking() });

            var result = await service.GetAllAsync();

            result.IsSucess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Bookings.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAnEmptyListOfReservations_GetAllAsync()
        {
            respositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<BookingEntity>());

            var result = await service.GetAllAsync();

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Bookings.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneBookingById_GetByIdAsync()
        {
            respositoryMock.Setup(_ => _.GetByIdAsync(BookingId))
                .ReturnsAsync(CreateBooking());

            var result = await service.GetByIdAsync(BookingId);

            result.IsSucess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task ShoulNotFindTheReservationById_GetByIdAsync()
        {
            var id = Guid.NewGuid();
            respositoryMock.Setup(_ => _.GetByIdAsync(id))
                .ReturnsAsync((BookingEntity)null!);

            var result = await service.GetByIdAsync(id);

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustBookRoom_BookRoomAsync()
        {
            var bookingRequest = CreateBookingRequest();
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Available);
            respositoryMock.Setup(_ => _.CreateAsync(It.IsAny<BookingEntity>()))
                .ReturnsAsync(1);

            var result = await service.BookRoomAsync(bookingRequest);

            result.IsSucess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
            respositoryMock.Verify(_ => _.CreateAsync(It.Is<BookingEntity>(booking =>
                booking.RoomId == bookingRequest.RoomId &&
                booking.CheckIn.Date == bookingRequest.CheckIn.Date &&
                booking.CheckOut.Date == bookingRequest.CheckOut.Date &&
                booking.GuestName == bookingRequest.GuestName)), Times.Once);
        }

        [Fact]
        public async Task MustTryBookRoomWithReservation_BookRoomAsync()
        {
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Booked);

            var result = await service.BookRoomAsync(CreateBookingRequest());

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date");
            respositoryMock.Verify(_ => _.CreateAsync(It.IsAny<BookingEntity>()), Times.Never);
        }

        [Fact]
        public async Task MustRejectEmptyRoomId_BookRoomAsync()
        {
            var bookingRequest = CreateBookingRequest();
            bookingRequest.RoomId = Guid.Empty;

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(bookingRequest));

            exception.Message.ShouldBe("RoomId is required.");
            respositoryMock.Verify(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task MustRejectEmptyGuestName_BookRoomAsync()
        {
            var bookingRequest = CreateBookingRequest();
            bookingRequest.GuestName = " ";

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(bookingRequest));

            exception.Message.ShouldBe("GuestName is required.");
            respositoryMock.Verify(_ => _.CreateAsync(It.IsAny<BookingEntity>()), Times.Never);
        }

        [Fact]
        public async Task MustUpdateBook_UpdateAsync()
        {
            var request = new UpdateBookingRequest
            {
                CheckIn = DateTime.Today.AddDays(16),
                CheckOut = DateTime.Today.AddDays(19),
                BookingId = BookingId
            };
            var booking = CreateBooking();
            respositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(booking);
            SetupAvailability(RoomStatusValueObject.Available);
            respositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(request);

            result.IsSucess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Id.ShouldBe(request.BookingId);
            result.Booking.CheckIn.Date.ShouldBe(request.CheckIn.Date);
            result.Booking.CheckOut.Date.ShouldBe(request.CheckOut.Date);
            result.Message.ShouldBeEmpty();
            respositoryMock.Verify(_ => _.UpdateAsync(booking), Times.Once);
        }

        [Fact]
        public async Task MustTryUpdateToADateWithReservation_UpdateAsync()
        {
            var request = new UpdateBookingRequest
            {
                CheckIn = DateTime.Today.AddDays(10),
                CheckOut = DateTime.Today.AddDays(12),
                BookingId = BookingId
            };
            respositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(CreateBooking());
            SetupAvailability(RoomStatusValueObject.Booked);

            var result = await service.UpdateAsync(request);

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date.");
            respositoryMock.Verify(_ => _.UpdateAsync(It.IsAny<BookingEntity>()), Times.Never);
        }

        [Fact]
        public async Task MustTryUpdatingANonExistingReservation_UpdateAsync()
        {
            var request = new UpdateBookingRequest
            {
                CheckIn = DateTime.Today.AddDays(10),
                CheckOut = DateTime.Today.AddDays(12),
                BookingId = Guid.NewGuid()
            };
            respositoryMock.Setup(_ => _.GetByIdAsync(request.BookingId))
                .ReturnsAsync((BookingEntity)null!);

            var result = await service.UpdateAsync(request);

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustRejectEmptyBookingId_UpdateAsync()
        {
            var request = new UpdateBookingRequest
            {
                CheckIn = DateTime.Today.AddDays(10),
                CheckOut = DateTime.Today.AddDays(12),
                BookingId = Guid.Empty
            };

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.UpdateAsync(request));

            exception.Message.ShouldBe("BookingId is required.");
            respositoryMock.Verify(_ => _.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task MustCancelBook_CancelAsync()
        {
            var booking = CreateBooking();
            respositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(booking);
            respositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>())).ReturnsAsync(1);

            var result = await service.CancelAsync(BookingId);

            result.IsSucess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Status.ShouldBe("BookingCanceled");
            result.Message.ShouldBe("Booking successfully canceled");
            respositoryMock.Verify(_ => _.UpdateAsync(booking), Times.Once);
        }

        [Fact]
        public async Task MustTryCancelANonExistingReservation_CancelAsync()
        {
            var bookingId = Guid.NewGuid();
            respositoryMock.Setup(_ => _.GetByIdAsync(bookingId))
                .ReturnsAsync((BookingEntity)null!);

            var result = await service.CancelAsync(bookingId);

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustValidateARoomWithoutReservation_CheckAvailabilityAsync()
        {
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Available);

            var result = await service.CheckAvailabilityAsync(CreateBookingRequest());

            result.IsSucess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Status.ShouldBe(RoomStatusValueObject.Available);
            result.Message.ShouldBe("Room available to book");
        }

        [Fact]
        public async Task MustValidateARoomWithReservation_CheckAvailabilityAsync()
        {
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Booked);

            var result = await service.CheckAvailabilityAsync(CreateBookingRequest());

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Status.ShouldBe(RoomStatusValueObject.Booked);
            result.Message.ShouldBe("Room not available for booking on this date");
        }

        [Fact]
        public async Task MustTryValidateANonExistingRoom_CheckAvailabilityAsync()
        {
            SetupRoomExists(false);

            var result = await service.CheckAvailabilityAsync(CreateBookingRequest());

            result.IsSucess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Status.ShouldBe(RoomStatusValueObject.None);
            result.Message.ShouldBe("Room not found");
        }

        [Fact]
        public async Task MustRejectEmptyRoomId_CheckAvailabilityAsync()
        {
            var bookingRequest = CreateBookingRequest();
            bookingRequest.RoomId = Guid.Empty;

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.CheckAvailabilityAsync(bookingRequest));

            exception.Message.ShouldBe("RoomId is required.");
            respositoryMock.Verify(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>()), Times.Never);
        }

        private void SetupRoomExists(bool exists)
        {
            respositoryMock.Setup(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>()))
                .ReturnsAsync(exists);
        }

        private void SetupAvailability(RoomStatusValueObject status)
        {
            respositoryMock.Setup(_ => _.CheckRoomAvailabilityAsync(RoomId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(status);
        }

        private static BookingRequest CreateBookingRequest()
        {
            return new BookingRequest
            {
                CheckIn = DateTime.Today.AddDays(16),
                CheckOut = DateTime.Today.AddDays(19),
                RoomId = RoomId,
                GuestName = "Elon Musk"
            };
        }

        private static BookingEntity CreateBooking()
        {
            return new BookingEntity(BookingId, DateTime.Today.AddDays(1), DateTime.Today.AddDays(4), RoomId, "Aegon Targaryen")
            {
                Room = new RoomEntity(RoomId, "MIO Cancun Hotel Boutique, Queen Suite")
            };
        }
    }
}
