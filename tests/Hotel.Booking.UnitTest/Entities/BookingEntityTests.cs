using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Exceptions;
using Shouldly;

namespace Hotel.Booking.Core.Entities.Tests
{
    public class BookingEntityTests
    {
        [Fact]
        public void ShouldCreateActiveBooking()
        {
            var booking = new BookingEntity(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), Guid.NewGuid(), "Test Guest");

            booking.Status.ShouldBe(BookingStatusValueObject.ActiveBooking);
        }

        [Fact]
        public void ShouldCancelBooking()
        {
            var booking = new BookingEntity(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), Guid.NewGuid(), "Test Guest");

            booking.Cancel();

            booking.Status.ShouldBe(BookingStatusValueObject.BookingCanceled);
        }

        [Fact]
        public void ShouldCheckOutActiveBooking()
        {
            var booking = new BookingEntity(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), Guid.NewGuid(), "Test Guest");

            booking.CheckOutRoom();

            booking.Status.ShouldBe(BookingStatusValueObject.CheckedOut);
        }

        [Fact]
        public void ShouldKeepCheckOutIdempotent()
        {
            var booking = new BookingEntity(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), Guid.NewGuid(), "Test Guest");

            booking.CheckOutRoom();
            booking.CheckOutRoom();

            booking.Status.ShouldBe(BookingStatusValueObject.CheckedOut);
        }

        [Fact]
        public void ShouldRejectCheckOutForCanceledBooking()
        {
            var booking = new BookingEntity(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), Guid.NewGuid(), "Test Guest");
            booking.Cancel();

            var exception = Should.Throw<BookingValidationException>(() => booking.CheckOutRoom());

            exception.Message.ShouldBe("Canceled booking cannot be checked out.");
            booking.Status.ShouldBe(BookingStatusValueObject.BookingCanceled);
        }

        [Fact]
        public void ShouldUpdateBookingDates()
        {
            var booking = new BookingEntity(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), Guid.NewGuid(), "Test Guest");
            var checkIn = DateTime.Today.AddDays(5);
            var checkOut = DateTime.Today.AddDays(7);

            booking.Update(checkIn, checkOut);

            booking.CheckIn.ShouldBe(checkIn);
            booking.CheckOut.ShouldBe(checkOut);
        }
    }
}
