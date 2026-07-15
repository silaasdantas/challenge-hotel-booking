using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.Interfaces
{
    public interface IRoomRepository
    {
        Task<List<RoomEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<RoomEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
