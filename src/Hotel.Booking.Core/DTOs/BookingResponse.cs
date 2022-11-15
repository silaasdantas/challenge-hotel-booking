using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.DTOs
{
    public class BookingResponse
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string GuestName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public RoomResponse Room { get; set; }
    }
}
