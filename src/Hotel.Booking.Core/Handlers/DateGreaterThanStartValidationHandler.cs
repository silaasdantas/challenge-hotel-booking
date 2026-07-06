using Hotel.Booking.Core.Interfaces;

using Hotel.Booking.Core.Exceptions;

namespace Hotel.Booking.Core.Handlers
{
    internal class DateGreaterThanStartValidationHandler : Handler, IHandler
    {
        public override void Handle(DateTime checkIn, DateTime checkOut)
        {
            if (checkOut.Date < checkIn.Date)
                throw new BookingValidationException("The end date must be greater than the start date.");

            base.Handle(checkIn, checkOut);
        }
    }
}
