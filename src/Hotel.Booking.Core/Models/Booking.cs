namespace Hotel.Booking.Core.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public String Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Room Room { get; set; }
    }
}
