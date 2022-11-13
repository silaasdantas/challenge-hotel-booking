using Hotel.Booking.Core.Models;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSucess, Models.Booking? Booking, string Message)> BookRoomAsync(CreateBookingCommand command);
        Task<(bool IsSucess, Models.Booking? Booking, string Message)> CancelAsync(Guid bookingId);
        Task<(bool IsSucess, IList<Models.Booking>? Bookings, string Message)> GetAllBookingAsync();
        Task<(bool IsSucess, Models.Booking? Booking, string Message)> GetBookingByIdAsync(Guid bookingId);
        Task<(bool IsSucess, Models.Booking? Booking, string Message)> UpdateBookingAsync(UpdateBookingCommand command);
    }
}
