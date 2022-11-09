using Hotel.Booking.Core.Models;
using Hotel.Booking.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Booking.Core.Services
{
    public class BookingService : IBookingService
    {
        public async Task<(bool IsSucess, Room room, string errorMessage)> 
            CheckRoomAvailability(DateTime checkIn, DateTime checkOut)
        {
            try
            {

                return (true, null, "");
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
