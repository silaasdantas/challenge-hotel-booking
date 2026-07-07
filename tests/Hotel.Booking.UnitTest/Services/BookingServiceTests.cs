using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Exceptions;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Results;
using Moq;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> repositoryMock;
        private readonly BookingService service;

        private static readonly Guid RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd");
        private static readonly Guid BookingId = Guid.Parse("d234a714-2298-4b7d-a957-cc4c3cc28786");

        public BookingServiceTests()
        {
            repositoryMock = new Mock<IBookingRepository>();

            service = new BookingService(repositoryMock.Object);
        }

        [Fact]
        public async Task MustReturnAllReservations_GetAllAsync()
        {
            repositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<BookingEntity> { CreateBooking() });

            var result = await service.GetAllAsync();

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Bookings.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAnEmptyListOfReservations_GetAllAsync()
        {
            repositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<BookingEntity>());

            var result = await service.GetAllAsync();

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Bookings.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneBookingById_GetByIdAsync()
        {
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId))
                .ReturnsAsync(CreateBooking());

            var result = await service.GetByIdAsync(BookingId);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task ShoulNotFindTheReservationById_GetByIdAsync()
        {
            var id = Guid.NewGuid();
            repositoryMock.Setup(_ => _.GetByIdAsync(id))
                .ReturnsAsync((BookingEntity)null!);

            var result = await service.GetByIdAsync(id);

            result.IsSuccess.ShouldBeFalse();
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
            repositoryMock.Setup(_ => _.CreateAsync(It.IsAny<BookingEntity>()))
                .ReturnsAsync(1);

            var result = await service.BookRoomAsync(bookingRequest);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
            repositoryMock.Verify(_ => _.CreateAsync(It.Is<BookingEntity>(booking =>
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

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date");
            repositoryMock.Verify(_ => _.CreateAsync(It.IsAny<BookingEntity>()), Times.Never);
        }

        [Fact]
        public async Task MustRejectEmptyRoomId_BookRoomAsync()
        {
            var bookingRequest = CreateBookingRequest();
            bookingRequest.RoomId = Guid.Empty;

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(bookingRequest));

            exception.Message.ShouldBe("RoomId is required.");
            repositoryMock.Verify(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task MustRejectEmptyGuestName_BookRoomAsync()
        {
            var bookingRequest = CreateBookingRequest();
            bookingRequest.GuestName = " ";

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(bookingRequest));

            exception.Message.ShouldBe("GuestName is required.");
            repositoryMock.Verify(_ => _.CreateAsync(It.IsAny<BookingEntity>()), Times.Never);
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
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(booking);
            SetupAvailability(RoomStatusValueObject.Available);
            repositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(request);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Id.ShouldBe(request.BookingId);
            result.Booking.CheckIn.Date.ShouldBe(request.CheckIn.Date);
            result.Booking.CheckOut.Date.ShouldBe(request.CheckOut.Date);
            result.Message.ShouldBeEmpty();
            repositoryMock.Verify(_ => _.CheckRoomAvailabilityAsync(
                booking.RoomId,
                request.CheckIn,
                request.CheckOut,
                booking.Id), Times.Once);
            repositoryMock.Verify(_ => _.UpdateAsync(booking), Times.Once);
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
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(CreateBooking());
            SetupAvailability(RoomStatusValueObject.Booked);

            var result = await service.UpdateAsync(request);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date.");
            repositoryMock.Verify(_ => _.UpdateAsync(It.IsAny<BookingEntity>()), Times.Never);
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
            repositoryMock.Setup(_ => _.GetByIdAsync(request.BookingId))
                .ReturnsAsync((BookingEntity)null!);

            var result = await service.UpdateAsync(request);

            result.IsSuccess.ShouldBeFalse();
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
            repositoryMock.Verify(_ => _.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task MustCancelBook_CancelAsync()
        {
            var booking = CreateBooking();
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(booking);
            repositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>())).ReturnsAsync(1);

            var result = await service.CancelAsync(BookingId);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Status.ShouldBe("BookingCanceled");
            result.Message.ShouldBe("Booking successfully canceled");
            repositoryMock.Verify(_ => _.UpdateAsync(booking), Times.Once);
        }

        [Fact]
        public async Task MustTryCancelANonExistingReservation_CancelAsync()
        {
            var bookingId = Guid.NewGuid();
            repositoryMock.Setup(_ => _.GetByIdAsync(bookingId))
                .ReturnsAsync((BookingEntity)null!);

            var result = await service.CancelAsync(bookingId);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustCheckOutBooking_CheckOutAsync()
        {
            var booking = CreateBooking();
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(booking);
            repositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>())).ReturnsAsync(1);

            var result = await service.CheckOutAsync(BookingId);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Status.ShouldBe("CheckedOut");
            result.Message.ShouldBe("Booking successfully checked out");
            repositoryMock.Verify(_ => _.UpdateAsync(booking), Times.Once);
        }

        [Fact]
        public async Task MustReturnSuccessForAlreadyCheckedOutBooking_CheckOutAsync()
        {
            var booking = CreateBooking();
            booking.CheckOutRoom();
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(booking);
            repositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>())).ReturnsAsync(1);

            var result = await service.CheckOutAsync(BookingId);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Status.ShouldBe("CheckedOut");
            result.Message.ShouldBe("Booking successfully checked out");
            repositoryMock.Verify(_ => _.UpdateAsync(booking), Times.Once);
        }

        [Fact]
        public async Task MustTryCheckOutNonExistingBooking_CheckOutAsync()
        {
            var bookingId = Guid.NewGuid();
            repositoryMock.Setup(_ => _.GetByIdAsync(bookingId))
                .ReturnsAsync((BookingEntity)null!);

            var result = await service.CheckOutAsync(bookingId);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
            repositoryMock.Verify(_ => _.UpdateAsync(It.IsAny<BookingEntity>()), Times.Never);
        }

        [Fact]
        public async Task MustRejectCanceledBooking_CheckOutAsync()
        {
            var booking = CreateBooking();
            booking.Cancel();
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId)).ReturnsAsync(booking);

            var result = await service.CheckOutAsync(BookingId);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.ValidationError);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Canceled booking cannot be checked out.");
            repositoryMock.Verify(_ => _.UpdateAsync(It.IsAny<BookingEntity>()), Times.Never);
        }

        [Fact]
        public async Task MustBlockSameDayBookingWhenPreviousBookingWasNotCheckedOut_BookRoomAsync()
        {
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Booked);

            var result = await service.BookRoomAsync(CreateBookingRequest());

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date");
            repositoryMock.Verify(_ => _.CreateAsync(It.IsAny<BookingEntity>()), Times.Never);
        }

        [Fact]
        public async Task MustAllowSameDayBookingAfterPreviousBookingWasCheckedOut_BookRoomAsync()
        {
            var bookingRequest = CreateBookingRequest();
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Available);
            repositoryMock.Setup(_ => _.CreateAsync(It.IsAny<BookingEntity>()))
                .ReturnsAsync(1);

            var result = await service.BookRoomAsync(bookingRequest);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            repositoryMock.Verify(_ => _.CreateAsync(It.IsAny<BookingEntity>()), Times.Once);
        }

        [Fact]
        public async Task MustValidateARoomWithoutReservation_CheckAvailabilityAsync()
        {
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Available);

            var result = await service.CheckAvailabilityAsync(CreateBookingRequest());

            result.IsSuccess.ShouldBeTrue();
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

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Status.ShouldBe(RoomStatusValueObject.Booked);
            result.Message.ShouldBe("Room not available for booking on this date");
        }

        [Fact]
        public async Task MustTryValidateANonExistingRoom_CheckAvailabilityAsync()
        {
            SetupRoomExists(false);

            var result = await service.CheckAvailabilityAsync(CreateBookingRequest());

            result.IsSuccess.ShouldBeFalse();
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
            repositoryMock.Verify(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>()), Times.Never);
        }

        private void SetupRoomExists(bool exists)
        {
            repositoryMock.Setup(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>()))
                .ReturnsAsync(exists);
        }

        private void SetupAvailability(RoomStatusValueObject status)
        {
            repositoryMock.Setup(_ => _.CheckRoomAvailabilityAsync(RoomId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>()))
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
