using AutoMapper;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Models;
using Microsoft.Extensions.Logging;

namespace Hotel.Booking.Core.Services
{
    public class RoomService : IRoomService
    {
        private readonly ILogger<RoomService> _logger;
        public readonly IEfRepository<RoomEntity> _repository;
        public readonly IEfRepository<BookingEntity> _repositoryBooking;
        public readonly IRoomRespository _roomRespository;
        private readonly IMapper _mapper;

        public RoomService(IEfRepository<RoomEntity> repository, 
            IEfRepository<BookingEntity> repositoryBooking,
            IRoomRespository roomRespository,
            ILogger<RoomService> logger, 
            IMapper mapper)
        {
            _repository = repository;
            _roomRespository = roomRespository;
            _repositoryBooking = repositoryBooking;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<(bool IsSucess, IList<Room> Rooms, string Message)>
            GetAllRoomsActivesAsync()
        {
            try
            {
                var rooms = await _repository.GetAsync(_ => _.IsActive);
                if (rooms != null && rooms.Any())
                {
                    var result = _mapper.Map<IList<RoomEntity>, IList<Room>>(rooms);
                    return (true, result, string.Empty);
                }
                return (false, new List<Room>(), "Not found");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return (false, new List<Room>(), ex.Message);
            }
        }

        public async Task<(bool IsSucess, string Status, string Message)>
            CheckRoomAvailabilityAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            try
            {
                var room = await _repository.GetByIdAsync(roomId);
                if (room == null)
                    return (false, string.Empty, "Room not found");

                var result = await _roomRespository.CheckRoomAvailabilityAsync(roomId, checkIn, checkOut);

                if (result.Equals(RoomStatusValueObject.Available))
                    return (true, RoomStatusValueObject.Available.ToString(), "Room available to book.");
                else
                    return (true, RoomStatusValueObject.Booked.ToString(), "Room not available for booking on this date.");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return (false, "Unable to load room data.", ex.Message);
            }
        }
    }
}
