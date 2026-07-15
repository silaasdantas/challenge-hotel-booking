using Hotel.Booking.Core.Interfaces;

using Hotel.Booking.Core.Exceptions;

namespace Hotel.Booking.Core.Handlers
{
    internal class StayLimitValidationHandler : Handler, IHandler
    {

        public override void Handle(DateTime checkIn, DateTime checkOut)
        {
            var stayLimit = 3;

            if ((checkOut.Date - checkIn.Date).Days > stayLimit)
                throw new BookingValidationException($"Rooms can`t be reserved for more than {stayLimit} days.");

            base.Handle(checkIn, checkOut);
        }
    }
}
