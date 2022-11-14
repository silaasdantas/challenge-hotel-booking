using Hotel.Booking.Core.Interfaces;

namespace Hotel.Booking.Core.Handlers
{
    internal class NextDayOfBookingValidationHandler : Handler, IHandler
    {
        public override void Handle(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn.Date <= DateTime.Now)
                throw new Exception("All reservations must start at least the next day of booking.");
            
            base.Handle(checkIn, checkOut);
        }
    }
}
