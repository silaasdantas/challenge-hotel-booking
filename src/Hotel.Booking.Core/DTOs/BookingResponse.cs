namespace Hotel.Booking.Core.DTOs
{
    public class BookingResponse
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public String Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public RoomResponse Room { get; set; }
    }
}
