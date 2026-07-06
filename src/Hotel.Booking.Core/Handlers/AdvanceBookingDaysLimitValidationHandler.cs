using Hotel.Booking.Core.Interfaces;

using Hotel.Booking.Core.Exceptions;

namespace Hotel.Booking.Core.Handlers
{
    internal class AdvanceBookingDaysLimitValidationHandler : Handler, IHandler
    {
        public override void Handle(DateTime checkIn, DateTime checkOut)
        {
            var advanceBookingDaysLimit = 30;

            if ((DateTime.Today.AddDays(advanceBookingDaysLimit) - checkIn.Date).Days <= -1)
                throw new BookingValidationException($"Rooms can`t be reserved more than {advanceBookingDaysLimit} days in advance.");

            base.Handle(checkIn, checkOut);
        }
    }
}
