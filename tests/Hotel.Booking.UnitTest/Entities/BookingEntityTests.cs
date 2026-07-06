using Hotel.Booking.Core.Entities;
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
