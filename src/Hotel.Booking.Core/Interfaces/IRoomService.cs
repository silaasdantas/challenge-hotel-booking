using Hotel.Booking.Core.DTOs;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IRoomService
    {
        Task<(bool IsSucess, RoomResponse? Room, string Message)> GetByIdAsync(Guid id);
        Task<(bool IsSucess, List<RoomResponse> Rooms, string Message)> GetAllRoomsActivesAsync();
    }
}
