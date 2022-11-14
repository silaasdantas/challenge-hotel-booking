using Hotel.Booking.Core.DTOs;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request);
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId);
        Task<(bool IsSucess, List<BookingResponse> Bookings, string Message)> GetAllBookingAsync();
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> GetBookingByIdAsync(Guid bookingId);
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> UpdateBookingAsync(UpdateBookingRequest request);
    }
}
