using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Mappers;

namespace Hotel.Booking.Core.Services
{
    public class RoomService : IRoomService
    {
        public readonly IRoomRepository _repository;

        public RoomService(IRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool IsSuccess, List<RoomResponse> Rooms, string Message)> GetAllRoomsActivesAsync()
        {
            try
            {
                var rooms = await _repository.GetAllAsync();
                if (rooms != null && rooms.Any())
                {
                    var result = ResponseMapper.ToRoomResponseList(rooms);
                    return (true, result, string.Empty);
                }
                return (false, new List<RoomResponse>(), "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSuccess, RoomResponse? Room, string Message)> GetByIdAsync(Guid id)
        {
            try
            {
                var room = await _repository.GetByIdAsync(id);
                if (room != null)
                    return (true, ResponseMapper.ToRoomResponse(room), string.Empty);

                return (false, null, Message: "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
