using Hotel.Booking.Core.Handlers;

namespace Hotel.Booking.Core.Validators
{
    public static class BookingDateValidator
    {
        public static void Validate(DateTime checkIn, DateTime checkOut)
        {
            var handler = new NextDayOfBookingValidationHandler();
            handler.SetNext(new DateGreaterThanStartValidationHandler())
                .SetNext(new AdvanceBookingDaysLimitValidationHandler())
                .SetNext(new StayLimitValidationHandler());

            handler.Handle(checkIn, checkOut);
        }
    }
}
