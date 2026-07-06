namespace Hotel.Booking.Core.Exceptions
{
    public class BookingValidationException : Exception
    {
        public BookingValidationException(string message)
            : base(message)
        {
        }
    }
}
