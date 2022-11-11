using Hotel.Booking.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Hotel.Booking.Infra.Data
{
    public class BookingRepository
    {
        public readonly BookingContext dbContext;
        public BookingRepository(BookingContext dbContext)
        {
            this.dbContext = dbContext;
        }

       

        public async Task<(bool IsSucess, IEnumerable<Core.Models.BookingEntity> Rooms, string ErrorMessage)> CheckRoomsAvailability(DateTime checkIn, DateTime checkOut)
        {
            try
            {
                //var bookings = await dbContext.Reservation.Where(_ => _.CheckIn.Date <= checkOut.Date && _.CheckOut <= checkIn).ToListAsync();
                //if (customers != null && customers.Any())
                //{
                //    var result = mapper.Map<IEnumerable<CustomerEntity>, IEnumerable<Customer>>(customers);
                //    return (true, result, null);
                //}
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                //logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

    }
}
