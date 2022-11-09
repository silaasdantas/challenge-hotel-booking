using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Booking.Core.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int AccommodationCapacity { get; set; }
        public int AdvanceBookingDaysLimit { get; set; }
        public int StayLimit { get; set; }
        public bool IsActive { get; set; }
        public List<Booking> Bookings { get; set; }

        public Room(string name, int accommodationCapacity, int advanceBookingDaysLimit, int stayLimit, bool isActive)
        {
            Id = Guid.NewGuid();
            Name = name;
            AccommodationCapacity = accommodationCapacity;
            AdvanceBookingDaysLimit = advanceBookingDaysLimit;
            StayLimit = stayLimit;
            IsActive = isActive;
            Bookings = new List<Booking>();
        }

        public void AddBooking(Booking booking)
        {
            Bookings.Add(booking);
        }
        public void CancelBooking(Booking booking)
        {
            Bookings.Add(booking);
        }

        public bool IsValidBooking(DateTime startDate, DateTime endDate)
        {
            //var stayLimit = 3;
            //var advanceReservationlimit = 30;
            var today = DateTime.Now;

            if (startDate.AddDays(1).Date <= today.Date)
                return false;
            //throw new Exception("All reservations must start at least the next day of booking.");

            if (endDate.Date < startDate.Date)
                return false;
            //throw new Exception("The end date must be greater than the start date.");

            if ((today.AddDays(AdvanceBookingDaysLimit).Date - startDate.Date).Days <= -1)
                return false;
            //throw new Exception($"Rooms can`t be reserved more than {advanceReservationlimit} days in advance.");

            if ((endDate.Date - startDate.Date).Days > StayLimit)
                return false;
            //throw new Exception($"Rooms can`t be reserved for more than {stayLimit} days.");

            return true;
        }
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



    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public Room Room { get; set; }

        public Booking(DateTime checkIn, DateTime checkOut, int adults, int children, Room room)
        {
            Id = Guid.NewGuid();
            CheckIn = checkIn;
            CheckOut = checkOut;
            Adults = adults;
            Children = children;
            Room = room;
        }
    }

    public enum BookingStatus
    {
        FreeRoom = 1,
        RoomBooked = 2,
        BookingCanceled = 3
    }
}
