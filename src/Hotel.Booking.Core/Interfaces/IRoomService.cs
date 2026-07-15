using Hotel.Booking.Core.DTOs;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IRoomService
    {
        Task<(bool IsSuccess, RoomResponse? Room, string Message)> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<(bool IsSuccess, List<RoomResponse> Rooms, string Message)> GetAllRoomsActivesAsync(CancellationToken cancellationToken);
    }
}
