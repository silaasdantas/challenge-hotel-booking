using Hotel.Booking.Core.Entities;
using System.Linq.Expressions;

namespace Hotel.Booking.Core.Specifications
{
    public static class BookingAvailabilitySpecification
    {
        public static Expression<Func<BookingEntity, bool>> ConflictsWith(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var requestedCheckIn = checkIn.Date;
            var requestedCheckOut = checkOut.Date;

            return booking => booking.RoomId.Equals(roomId)
                && booking.Room.IsActive
                && booking.Status.Equals(BookingStatusValueObject.ActiveBooking)
                && booking.CheckIn.Date <= requestedCheckOut
                && booking.CheckOut.Date >= requestedCheckIn;
        }
    }
}
