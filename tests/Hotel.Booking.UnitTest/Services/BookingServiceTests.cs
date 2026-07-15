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
            repositoryMock.Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BookingEntity> { CreateBooking() });

            var result = await service.GetAllAsync(CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Bookings.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnNotFound_WhenReservationsAreEmpty_GetAllAsync()
        {
            repositoryMock.Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BookingEntity>());

            var result = await service.GetAllAsync(CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Bookings.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneBookingById_GetByIdAsync()
        {
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateBooking());

            var result = await service.GetByIdAsync(BookingId, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnNotFound_WhenBookingDoesNotExist_GetByIdAsync()
        {
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookingEntity?)null);

            var result = await service.GetByIdAsync(BookingId, CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustBookRoom_BookRoomAsync()
        {
            var request = CreateBookingRequest();
            SetupRoomExists(true);
            repositoryMock.Setup(_ => _.TryCreateBookingAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, CreateBooking(request)));

            var result = await service.BookRoomAsync(request, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
            repositoryMock.Verify(_ => _.TryCreateBookingAsync(It.Is<BookingEntity>(booking =>
                booking.RoomId == request.RoomId &&
                booking.CheckIn.Date == request.CheckIn.Date &&
                booking.CheckOut.Date == request.CheckOut.Date &&
                booking.GuestName == request.GuestName), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MustReturnConflict_WhenAtomicCreateFindsReservation_BookRoomAsync()
        {
            SetupRoomExists(true);
            repositoryMock.Setup(_ => _.TryCreateBookingAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, CreateBooking()));

            var result = await service.BookRoomAsync(CreateBookingRequest(), CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date");
        }

        [Fact]
        public async Task MustReturnNotFound_WhenRoomDoesNotExist_BookRoomAsync()
        {
            SetupRoomExists(false);

            var result = await service.BookRoomAsync(CreateBookingRequest(), CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not found");
            repositoryMock.Verify(_ => _.TryCreateBookingAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task MustRejectEmptyRoomId_BookRoomAsync()
        {
            var request = CreateBookingRequest();
            request.RoomId = Guid.Empty;

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(request, CancellationToken.None));

            exception.Message.ShouldBe("RoomId is required.");
            repositoryMock.Verify(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task MustRejectEmptyGuestName_BookRoomAsync()
        {
            var request = CreateBookingRequest();
            request.GuestName = " ";

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.BookRoomAsync(request, CancellationToken.None));

            exception.Message.ShouldBe("GuestName is required.");
            repositoryMock.Verify(_ => _.TryCreateBookingAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task MustUpdateBook_UpdateAsync()
        {
            var request = CreateUpdateRequest();
            var updatedBooking = CreateBooking(request);
            repositoryMock.Setup(_ => _.TryUpdateBookingDatesAsync(request.BookingId, request.CheckIn, request.CheckOut, It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, false, updatedBooking));

            var result = await service.UpdateAsync(request, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Id.ShouldBe(request.BookingId);
            result.Booking.CheckIn.Date.ShouldBe(request.CheckIn.Date);
            result.Booking.CheckOut.Date.ShouldBe(request.CheckOut.Date);
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnConflict_WhenAtomicUpdateFindsReservation_UpdateAsync()
        {
            var request = CreateUpdateRequest();
            repositoryMock.Setup(_ => _.TryUpdateBookingDatesAsync(request.BookingId, request.CheckIn, request.CheckOut, It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, false, null));

            var result = await service.UpdateAsync(request, CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Room not available for booking on this date.");
        }

        [Fact]
        public async Task MustReturnNotFound_WhenAtomicUpdateDoesNotFindBooking_UpdateAsync()
        {
            var request = CreateUpdateRequest();
            repositoryMock.Setup(_ => _.TryUpdateBookingDatesAsync(request.BookingId, request.CheckIn, request.CheckOut, It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, true, null));

            var result = await service.UpdateAsync(request, CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustRejectEmptyBookingId_UpdateAsync()
        {
            var request = CreateUpdateRequest();
            request.BookingId = Guid.Empty;

            var exception = await Should.ThrowAsync<BookingValidationException>(() => service.UpdateAsync(request, CancellationToken.None));

            exception.Message.ShouldBe("BookingId is required.");
            repositoryMock.Verify(_ => _.TryUpdateBookingDatesAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task MustCancelBook_CancelAsync()
        {
            var booking = CreateBooking();
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            repositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CancelAsync(BookingId, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Status.ShouldBe("BookingCanceled");
            result.Message.ShouldBe("Booking successfully canceled");
            repositoryMock.Verify(_ => _.UpdateAsync(booking, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MustCheckOutBooking_CheckOutAsync()
        {
            var booking = CreateBooking();
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
            repositoryMock.Setup(_ => _.UpdateAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CheckOutAsync(BookingId, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.StatusResult.ShouldBe(ServiceResultStatus.Success);
            result.Booking.ShouldNotBeNull();
            result.Booking.Status.ShouldBe("CheckedOut");
            result.Message.ShouldBe("Booking successfully checked out");
        }

        [Fact]
        public async Task MustRejectCanceledBooking_CheckOutAsync()
        {
            var booking = CreateBooking();
            booking.Cancel();
            repositoryMock.Setup(_ => _.GetByIdAsync(BookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

            var result = await service.CheckOutAsync(BookingId, CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.ValidationError);
            result.Booking.ShouldBeNull();
            result.Message.ShouldBe("Canceled booking cannot be checked out.");
            repositoryMock.Verify(_ => _.UpdateAsync(It.IsAny<BookingEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task MustValidateARoomWithoutReservation_CheckAvailabilityAsync()
        {
            SetupRoomExists(true);
            SetupAvailability(RoomStatusValueObject.Available);

            var result = await service.CheckAvailabilityAsync(CreateAvailabilityRequest(), CancellationToken.None);

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

            var result = await service.CheckAvailabilityAsync(CreateAvailabilityRequest(), CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.Conflict);
            result.Status.ShouldBe(RoomStatusValueObject.Booked);
            result.Message.ShouldBe("Room not available for booking on this date");
        }

        [Fact]
        public async Task MustTryValidateANonExistingRoom_CheckAvailabilityAsync()
        {
            SetupRoomExists(false);

            var result = await service.CheckAvailabilityAsync(CreateAvailabilityRequest(), CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.StatusResult.ShouldBe(ServiceResultStatus.NotFound);
            result.Status.ShouldBe(RoomStatusValueObject.None);
            result.Message.ShouldBe("Room not found");
        }

        [Fact]
        public async Task MustPassCancellationToken_ToRepository()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            repositoryMock.Setup(_ => _.GetAllAsync(token)).ReturnsAsync(new List<BookingEntity>());

            await service.GetAllAsync(token);

            repositoryMock.Verify(_ => _.GetAllAsync(token), Times.Once);
        }

        private void SetupRoomExists(bool exists)
        {
            repositoryMock.Setup(_ => _.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BookingEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(exists);
        }

        private void SetupAvailability(RoomStatusValueObject status)
        {
            repositoryMock.Setup(_ => _.CheckRoomAvailabilityAsync(RoomId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
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

        private static AvailabilityRequest CreateAvailabilityRequest()
        {
            return new AvailabilityRequest
            {
                CheckIn = DateTime.Today.AddDays(16),
                CheckOut = DateTime.Today.AddDays(19),
                RoomId = RoomId
            };
        }

        private static UpdateBookingRequest CreateUpdateRequest()
        {
            return new UpdateBookingRequest
            {
                CheckIn = DateTime.Today.AddDays(16),
                CheckOut = DateTime.Today.AddDays(19),
                BookingId = BookingId
            };
        }

        private static BookingEntity CreateBooking()
        {
            return new BookingEntity(BookingId, DateTime.Today.AddDays(1), DateTime.Today.AddDays(4), RoomId, "Aegon Targaryen")
            {
                Room = new RoomEntity(RoomId, "MIO Cancun Hotel Boutique, Queen Suite")
            };
        }

        private static BookingEntity CreateBooking(BookingRequest request)
        {
            return new BookingEntity(BookingId, request.CheckIn, request.CheckOut, request.RoomId, request.GuestName)
            {
                Room = new RoomEntity(request.RoomId, "MIO Cancun Hotel Boutique, Queen Suite")
            };
        }

        private static BookingEntity CreateBooking(UpdateBookingRequest request)
        {
            return new BookingEntity(request.BookingId, request.CheckIn, request.CheckOut, RoomId, "Aegon Targaryen")
            {
                Room = new RoomEntity(RoomId, "MIO Cancun Hotel Boutique, Queen Suite")
            };
        }
    }
}
