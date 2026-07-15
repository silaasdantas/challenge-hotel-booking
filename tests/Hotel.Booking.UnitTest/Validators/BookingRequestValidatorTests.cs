using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Exceptions;
using Hotel.Booking.Core.Validators;
using Shouldly;

namespace Hotel.Booking.Core.Validators.Tests
{
    public class BookingRequestValidatorTests
    {
        [Fact]
        public void ShouldReject_WhenRoomIdIsEmpty()
        {
            var request = CreateValidBookingRequest();
            request.RoomId = Guid.Empty;

            var exception = Should.Throw<BookingValidationException>(() => BookingRequestValidator.ValidateForBooking(request));

            exception.Message.ShouldBe("RoomId is required.");
        }

        [Fact]
        public void ShouldReject_WhenCheckInIsDefault()
        {
            var request = CreateValidBookingRequest();
            request.CheckIn = default;

            var exception = Should.Throw<BookingValidationException>(() => BookingRequestValidator.ValidateForBooking(request));

            exception.Message.ShouldBe("CheckIn is required.");
        }

        [Fact]
        public void ShouldReject_WhenCheckOutIsDefault()
        {
            var request = CreateValidBookingRequest();
            request.CheckOut = default;

            var exception = Should.Throw<BookingValidationException>(() => BookingRequestValidator.ValidateForBooking(request));

            exception.Message.ShouldBe("CheckOut is required.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ShouldReject_WhenGuestNameIsInvalid(string? guestName)
        {
            var request = CreateValidBookingRequest();
            request.GuestName = guestName!;

            var exception = Should.Throw<BookingValidationException>(() => BookingRequestValidator.ValidateForBooking(request));

            exception.Message.ShouldBe("GuestName is required.");
        }

        [Fact]
        public void ShouldAcceptAvailability_WhenGuestNameIsMissing()
        {
            var request = CreateValidBookingRequest();
            request.GuestName = null!;

            Should.NotThrow(() => BookingRequestValidator.ValidateForAvailability(request));
        }

        [Fact]
        public void ShouldAcceptAvailabilityRequest_WhenRequestIsValid()
        {
            var request = new AvailabilityRequest
            {
                RoomId = Guid.NewGuid(),
                CheckIn = DateTime.Today.AddDays(1),
                CheckOut = DateTime.Today.AddDays(3)
            };

            Should.NotThrow(() => BookingRequestValidator.ValidateForAvailability(request));
        }

        [Fact]
        public void ShouldAccept_WhenBookingRequestIsValid()
        {
            var request = CreateValidBookingRequest();

            Should.NotThrow(() => BookingRequestValidator.ValidateForBooking(request));
        }

        [Fact]
        public void ShouldRejectUpdate_WhenBookingIdIsEmpty()
        {
            var request = new UpdateBookingRequest
            {
                BookingId = Guid.Empty,
                CheckIn = DateTime.Today.AddDays(1),
                CheckOut = DateTime.Today.AddDays(3)
            };

            var exception = Should.Throw<BookingValidationException>(() => BookingRequestValidator.ValidateForUpdate(request));

            exception.Message.ShouldBe("BookingId is required.");
        }

        private static BookingRequest CreateValidBookingRequest()
        {
            return new BookingRequest
            {
                RoomId = Guid.NewGuid(),
                CheckIn = DateTime.Today.AddDays(1),
                CheckOut = DateTime.Today.AddDays(3),
                GuestName = "Ada Lovelace"
            };
        }
    }
}
