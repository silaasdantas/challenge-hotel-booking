using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Results;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IBookingService
    {
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request, CancellationToken cancellationToken);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId, CancellationToken cancellationToken);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> CheckOutAsync(Guid bookingId, CancellationToken cancellationToken);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, List<BookingResponse> Bookings, string Message)> GetAllAsync(CancellationToken cancellationToken);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> UpdateAsync(UpdateBookingRequest request, CancellationToken cancellationToken);
        Task<(bool IsSuccess, ServiceResultStatus StatusResult, RoomStatusValueObject Status, string Message)> CheckAvailabilityAsync(AvailabilityRequest request, CancellationToken cancellationToken);
    }
}
