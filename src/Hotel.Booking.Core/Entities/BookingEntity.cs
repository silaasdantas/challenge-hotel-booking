namespace Hotel.Booking.Core.Entities
{
    public class BookingEntity
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string GuestName { get; set; }
        public Guid RoomId { get; set; }
        public BookingStatusValueObject Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public RoomEntity Room { get; set; }

        public BookingEntity(DateTime checkIn, DateTime checkOut, Guid roomId, string guestName)
        {
            Id = Guid.NewGuid();
            CheckIn = checkIn;
            CheckOut = checkOut;
            RoomId = roomId;
            Status = BookingStatusValueObject.ActiveBooking;
            CreatedAt = DateTime.Now;
            GuestName = guestName;
        }

        public void Cancel()
        {
            Status = BookingStatusValueObject.BookingCanceled;
        }

        public void Update(DateTime checkIn, DateTime checkOut)
        {
            CheckIn = checkIn;
            CheckOut = checkOut;
        }
    }
}
