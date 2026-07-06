using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Specifications;
using Shouldly;

namespace Hotel.Booking.Core.Specifications.Tests
{
    public class BookingAvailabilitySpecificationTests
    {
        private readonly Guid roomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd");

        [Fact]
        public void ShouldConflict_WhenRangesAreEqual()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(roomId, new DateTime(2026, 7, 10), new DateTime(2026, 7, 12))
                .Compile();

            specification(booking).ShouldBeTrue();
        }

        [Fact]
        public void ShouldConflict_WhenRequestedRangeStartsInsideExistingBooking()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(roomId, new DateTime(2026, 7, 11), new DateTime(2026, 7, 13))
                .Compile();

            specification(booking).ShouldBeTrue();
        }

        [Fact]
        public void ShouldConflict_WhenRequestedRangeEndsInsideExistingBooking()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(roomId, new DateTime(2026, 7, 8), new DateTime(2026, 7, 11))
                .Compile();

            specification(booking).ShouldBeTrue();
        }

        [Fact]
        public void ShouldConflict_WhenRangeBordersAreEqual()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(roomId, new DateTime(2026, 7, 12), new DateTime(2026, 7, 14))
                .Compile();

            specification(booking).ShouldBeTrue();
        }

        [Fact]
        public void ShouldNotConflict_WhenRangesDoNotIntersect()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(roomId, new DateTime(2026, 7, 13), new DateTime(2026, 7, 15))
                .Compile();

            specification(booking).ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotConflict_WhenRoomIdIsDifferent()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(Guid.NewGuid(), new DateTime(2026, 7, 10), new DateTime(2026, 7, 12))
                .Compile();

            specification(booking).ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotConflict_WhenRoomIsInactive()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            booking.Room.IsActive = false;
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(roomId, new DateTime(2026, 7, 10), new DateTime(2026, 7, 12))
                .Compile();

            specification(booking).ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotConflict_WhenBookingIsCanceled()
        {
            var booking = CreateBooking(new DateTime(2026, 7, 10), new DateTime(2026, 7, 12));
            booking.Cancel();
            var specification = BookingAvailabilitySpecification
                .ConflictsWith(roomId, new DateTime(2026, 7, 10), new DateTime(2026, 7, 12))
                .Compile();

            specification(booking).ShouldBeFalse();
        }

        private BookingEntity CreateBooking(DateTime checkIn, DateTime checkOut)
        {
            var room = new RoomEntity("Test Room") { Id = roomId };
            return new BookingEntity(checkIn, checkOut, room.Id, "Test Guest")
            {
                Room = room
            };
        }
    }
}
