using Hotel.Booking.Core.Interfaces;

namespace Hotel.Booking.Core.Handlers
{
    internal class AdvanceBookingDaysLimitValidationHandler : Handler, IHandler
    {
        public override void Handle(DateTime checkIn, DateTime checkOut)
        {
            var advanceBookingDaysLimit = 30;

            if ((DateTime.Now.AddDays(advanceBookingDaysLimit).Date - checkIn.Date).Days <= -1)
                throw new Exception($"Rooms can`t be reserved more than {advanceBookingDaysLimit} days in advance.");

            base.Handle(checkIn, checkOut);
        }
    }
}
