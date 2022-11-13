using System.ComponentModel.DataAnnotations;

namespace Hotel.Booking.Core.Models
{
    public class UpdateBooking
    {
        [Required]
        public Guid BookingId { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
    }
}
