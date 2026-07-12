using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Results;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> CheckOutAsync(Guid bookingId);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, List<BookingResponse> Bookings, string Message)> GetAllAsync();
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> GetByIdAsync(Guid bookingId);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> UpdateAsync(UpdateBookingRequest request);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, RoomStatusValueObject Status, string Message)> CheckAvailabilityAsync(AvailabilityRequest request);
    }
}
