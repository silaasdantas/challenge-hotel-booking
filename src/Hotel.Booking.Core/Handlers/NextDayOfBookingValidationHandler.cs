using Hotel.Booking.Core.Interfaces;

using Hotel.Booking.Core.Exceptions;

namespace Hotel.Booking.Core.Handlers
{
    internal class NextDayOfBookingValidationHandler : Handler, IHandler
    {
        public override void Handle(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn.Date <= DateTime.Today)
                throw new BookingValidationException("All reservations must start at least the next day of booking.");
            
            base.Handle(checkIn, checkOut);
        }
    }
}
