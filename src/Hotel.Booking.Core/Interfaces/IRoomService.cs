using Hotel.Booking.Core.Models;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IRoomService
    {
        Task<(bool IsSucess, string Status, string Message)>
            CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut);

        Task<(bool IsSucess, IList<Room>? Rooms, string Message)>
            GetAllRoomsActivesAsync();
    }
}
