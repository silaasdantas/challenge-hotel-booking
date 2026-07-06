using Hotel.Booking.Core.Validators;
using Shouldly;

namespace Hotel.Booking.Core.Validators.Tests
{
    public class BookingDateValidatorTests
    {
        [Fact]
        public void ShouldAccept_WhenBookingStartsTomorrow()
        {
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);

            Should.NotThrow(() => BookingDateValidator.Validate(checkIn, checkOut));
        }

        [Fact]
        public void ShouldReject_WhenCheckInIsToday()
        {
            var checkIn = DateTime.Now;
            var checkOut = DateTime.Now.AddDays(2);

            var exception = Should.Throw<Exception>(() => BookingDateValidator.Validate(checkIn, checkOut));
            exception.Message.ShouldBe("All reservations must start at least the next day of booking.");
        }

        [Fact]
        public void ShouldReject_WhenCheckInIsInThePast()
        {
            var checkIn = DateTime.Now.AddDays(-1);
            var checkOut = DateTime.Now.AddDays(2);

            var exception = Should.Throw<Exception>(() => BookingDateValidator.Validate(checkIn, checkOut));
            exception.Message.ShouldBe("All reservations must start at least the next day of booking.");
        }

        [Fact]
        public void ShouldReject_WhenCheckOutIsBeforeCheckIn()
        {
            var checkIn = DateTime.Now.AddDays(5);
            var checkOut = DateTime.Now.AddDays(4);

            var exception = Should.Throw<Exception>(() => BookingDateValidator.Validate(checkIn, checkOut));
            exception.Message.ShouldBe("The end date must be greater than the start date.");
        }

        [Fact]
        public void ShouldReject_WhenBookingIsMoreThanThirtyDaysInAdvance()
        {
            var checkIn = DateTime.Now.AddDays(31);
            var checkOut = DateTime.Now.AddDays(33);

            var exception = Should.Throw<Exception>(() => BookingDateValidator.Validate(checkIn, checkOut));
            exception.Message.ShouldBe("Rooms can`t be reserved more than 30 days in advance.");
        }

        [Fact]
        public void ShouldReject_WhenStayIsLongerThanThreeDays()
        {
            var checkIn = DateTime.Now.AddDays(5);
            var checkOut = DateTime.Now.AddDays(9);

            var exception = Should.Throw<Exception>(() => BookingDateValidator.Validate(checkIn, checkOut));
            exception.Message.ShouldBe("Rooms can`t be reserved for more than 3 days.");
        }

        [Fact]
        public void ShouldAccept_WhenStayIsExactlyThreeDays()
        {
            var checkIn = DateTime.Now.AddDays(5);
            var checkOut = DateTime.Now.AddDays(8);

            Should.NotThrow(() => BookingDateValidator.Validate(checkIn, checkOut));
        }
    }
}
