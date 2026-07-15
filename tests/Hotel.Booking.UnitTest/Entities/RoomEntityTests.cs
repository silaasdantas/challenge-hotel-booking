using Hotel.Booking.Core.Entities;
using Shouldly;

namespace Hotel.Booking.Core.Entities.Tests
{
    public class RoomEntityTests
    {
        [Fact]
        public void ShouldCreateActiveRoom()
        {
            var room = new RoomEntity("Test Room");

            room.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void ShouldDeactivateRoom()
        {
            var room = new RoomEntity("Test Room");

            room.Deactivate();

            room.IsActive.ShouldBeFalse();
        }
    }
}
