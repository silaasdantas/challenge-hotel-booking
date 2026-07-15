namespace Hotel.Booking.Core.ValueObjects
{
    public class BookingDateRange
    {
        public DateTime CheckIn { get; }
        public DateTime CheckOut { get; }

        public BookingDateRange(DateTime checkIn, DateTime checkOut)
        {
            CheckIn = checkIn.Date;
            CheckOut = checkOut.Date;
        }

        public bool Overlaps(BookingDateRange other) =>
            CheckIn <= other.CheckOut && CheckOut >= other.CheckIn;
    }
}
