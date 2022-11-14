using Hotel.Booking.Core.DTOs;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IRoomService
    {
        Task<(bool IsSucess, string Status, string Message)>
        //CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
        CheckRoomAvailabilityAsync(BookingRequest search);

        Task<(bool IsSucess, IList<RoomResponse> Rooms, string Message)>
            GetAllRoomsActivesAsync();
    }
}
