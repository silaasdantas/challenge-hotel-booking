using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IRoomRespository
    {
        Task<RoomStatusValueObject> CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
        Task<List<RoomEntity>> GetAllAsync();
        Task<RoomEntity> GetByIdAsync(Guid id);
    }
}
