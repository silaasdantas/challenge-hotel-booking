using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;

namespace Hotel.Booking.Core.Services
{
    public class RoomService : IRoomService
    {
        public readonly IRoomRespository _respository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRespository respository,
            IMapper mapper)
        {
            _respository = respository;
            _mapper = mapper;
        }

        public async Task<(bool IsSucess, List<RoomResponse> Rooms, string Message)> GetAllRoomsActivesAsync()
        {
            try
            {
                var rooms = await _respository.GetAllAsync();
                if (rooms != null && rooms.Any())
                {
                    var result = _mapper.Map<List<RoomEntity>, List<RoomResponse>>(rooms);
                    return (true, result, string.Empty);
                }
                return (false, new List<RoomResponse>(), "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, RoomResponse? Room, string Message)> GetByIdAsync(Guid id)
        {
            try
            {
                var room = await _respository.GetByIdAsync(id);
                if (room != null)
                    return (true, _mapper.Map<RoomEntity, RoomResponse>(room), string.Empty);

                return (false, null, Message: "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
