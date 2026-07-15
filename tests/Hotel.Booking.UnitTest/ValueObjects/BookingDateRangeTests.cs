using Hotel.Booking.Core.ValueObjects;
using Shouldly;

namespace Hotel.Booking.Core.ValueObjects.Tests
{
    public class BookingDateRangeTests
    {
        [Fact]
        public void ShouldOverlap_WhenRangesAreEqual()
        {
            var current = new BookingDateRange(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var other = new BookingDateRange(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));

            current.Overlaps(other).ShouldBeTrue();
        }

        [Fact]
        public void ShouldOverlap_WhenOtherStartsInsideCurrentRange()
        {
            var current = new BookingDateRange(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var other = new BookingDateRange(new DateTime(2026, 7, 11), new DateTime(2026, 7, 13));

            current.Overlaps(other).ShouldBeTrue();
        }

        [Fact]
        public void ShouldOverlap_WhenOtherEndsInsideCurrentRange()
        {
            var current = new BookingDateRange(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var other = new BookingDateRange(new DateTime(2026, 7, 8), new DateTime(2026, 7, 11));

            current.Overlaps(other).ShouldBeTrue();
        }

        [Fact]
        public void ShouldOverlap_WhenOtherCoversCurrentRange()
        {
            var current = new BookingDateRange(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var other = new BookingDateRange(new DateTime(2026, 7, 8), new DateTime(2026, 7, 14));

            current.Overlaps(other).ShouldBeTrue();
        }

        [Fact]
        public void ShouldNotOverlap_WhenRangesDoNotIntersect()
        {
            var current = new BookingDateRange(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var other = new BookingDateRange(new DateTime(2026, 7, 13), new DateTime(2026, 7, 15));

            current.Overlaps(other).ShouldBeFalse();
        }

        [Fact]
        public void ShouldOverlap_WhenRangeBordersAreEqual()
        {
            var current = new BookingDateRange(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var other = new BookingDateRange(new DateTime(2026, 7, 12), new DateTime(2026, 7, 14));

            current.Overlaps(other).ShouldBeTrue();
        }
    }
}
