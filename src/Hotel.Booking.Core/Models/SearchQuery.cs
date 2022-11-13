using System.ComponentModel.DataAnnotations;

namespace Hotel.Booking.Core.Models
{
    public class SearchQuery
    {
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }
    }
}
