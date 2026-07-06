namespace Hotel.Booking.Core.Entities
{
    public class BookingEntity
    {
        public Guid Id { get; private set; }
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }
        public string GuestName { get; private set; }
        public Guid RoomId { get; private set; }
        public BookingStatusValueObject Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public RoomEntity Room { get; set; }

        public BookingEntity(DateTime checkIn, DateTime checkOut, Guid roomId, string guestName)
            : this(Guid.NewGuid(), checkIn, checkOut, roomId, guestName)
        {
        }

        public BookingEntity(Guid id, DateTime checkIn, DateTime checkOut, Guid roomId, string guestName)
        {
            Id = id;
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
