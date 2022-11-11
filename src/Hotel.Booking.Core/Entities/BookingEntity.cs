namespace Hotel.Booking.Core.Models
{
    public class BookingEntity
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public Guid RoomId { get; set; }
        public int AdvanceBookingDaysLimit { get; set; }
        public int StayLimit { get; set; }
        public BookingStatusValueObject BookingStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual RoomEntity Room { get; set; }

        public BookingEntity(DateTime checkIn, DateTime checkOut, Guid roomId)
        {
            Id = Guid.NewGuid();
            CheckIn = checkIn;
            CheckOut = checkOut;
            RoomId = roomId;
            StayLimit = 3;
            AdvanceBookingDaysLimit = 30;
            BookingStatus = BookingStatusValueObject.RoomBooked;
            CreatedAt = DateTime.Now;
        }

        public (bool IsSucess, string errorMessage) IsValidBooking(DateTime checkIn, DateTime checkOut)
        {
            var today = DateTime.Now;

            if (checkIn.Date <= CreatedAt.Date)
                return (false, "All reservations must start at least the next day of booking.");

            //if (CreatedAt.Date == startDate.Date)
            //    return (false, "All reservations must start at least the next day of booking.");

            if (checkOut.Date < checkIn.Date)
                return (false, "The end date must be greater than the start date.");

            if ((today.AddDays(AdvanceBookingDaysLimit).Date - checkIn.Date).Days <= -1)
                return (false, $"Rooms can`t be reserved more than {AdvanceBookingDaysLimit} days in advance.");

            if ((checkOut.Date - checkIn.Date).Days > StayLimit)
                return (false, $"Rooms can`t be reserved for more than {StayLimit} days.");

            return (true, "");
        }

        public void Cancel()
        {
            BookingStatus = BookingStatusValueObject.BookingCanceled;
        }

        public (bool IsSucess, string errorMessage) Update(DateTime checkIn, DateTime checkOut)
        {
            var isValid = IsValidBooking(checkIn, checkOut);
            if (isValid.IsSucess)
            {
                CheckIn = checkIn;
                CheckOut = checkOut;
                return (true, "");
            }

            return (false, isValid.errorMessage);
        }
    }
}
