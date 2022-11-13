namespace Hotel.Booking.Core.Models
{
    public class CreateBookingCommand
    {
        public Guid RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
