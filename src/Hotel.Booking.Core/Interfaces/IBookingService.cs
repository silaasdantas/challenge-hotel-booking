using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request);
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId);
        Task<(bool IsSucess, List<BookingResponse> Bookings, string Message)> GetAllAsync();
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> GetByIdAsync(Guid bookingId);
        Task<(bool IsSucess, BookingResponse? Booking, string Message)> UpdateAsync(UpdateBookingRequest request);
        Task<(bool IsSucess, RoomStatusValueObject Status, string Message)> CheckAvailabilityAsync(BookingRequest request);
    }
}
