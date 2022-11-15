using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IRoomRespository
    {
        Task<List<RoomEntity>> GetAllAsync();
        Task<RoomEntity> GetByIdAsync(Guid id);
    }
}
