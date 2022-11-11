namespace Hotel.Booking.Api
{
    public class CreateBookingCommand
    {
        public Guid RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }

    public class UpdateBookingCommand
    {
        public Guid BookingId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
