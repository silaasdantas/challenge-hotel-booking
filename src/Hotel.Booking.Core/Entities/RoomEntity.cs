namespace Hotel.Booking.Core.Entities
{
    public class RoomEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RoomStatusValueObject Status { get; set; }
        public bool IsActive { get; set; }
        //public ICollection<BookingEntity> Bookings { get; set; }

        public RoomEntity(string name)
        {
            Id = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd"); //Guid.NewGuid();
            Name = name;
            IsActive = true;
            Status = RoomStatusValueObject.Available;
            //Bookings = new List<BookingEntity>();
        }

        public void RoomBooked()
        {
            Status = RoomStatusValueObject.Booked;
        }
        public void RoomAvailable()
        {
            Status = RoomStatusValueObject.Available;
        }

        //public void AddBooking(BookingEntity booking)
        //{
        //    Bookings.Add(booking);
        //}
        //public void CancelBooking(BookingEntity booking)
        //{
        //    Bookings.Add(booking);
        //}

        //public bool IsValidBooking(DateTime startDate, DateTime endDate)
        //{
        //    //var stayLimit = 3;
        //    //var advanceReservationlimit = 30;
        //    var today = DateTime.Now;

        //    if (startDate.AddDays(1).Date <= today.Date)
        //        return false;
        //    //throw new Exception("All reservations must start at least the next day of booking.");

        //    if (endDate.Date < startDate.Date)
        //        return false;
        //    //throw new Exception("The end date must be greater than the start date.");

        //    if ((today.AddDays(AdvanceBookingDaysLimit).Date - startDate.Date).Days <= -1)
        //        return false;
        //    //throw new Exception($"Rooms can`t be reserved more than {advanceReservationlimit} days in advance.");

        //    if ((endDate.Date - startDate.Date).Days > StayLimit)
        //        return false;
        //    //throw new Exception($"Rooms can`t be reserved for more than {stayLimit} days.");

        //    return true;
        //}
    }
    //public class Hotel
    //{
    //    public Guid Id { get; set; }
    //    public string Name { get; set; }
    //    public List<Room> Rooms { get; set; }
    //    public Hotel(string name)
    //    {
    //        Id = Guid.NewGuid();
    //        Name = name;
    //        Rooms = new List<Room>();
    //    }
    //}

}
