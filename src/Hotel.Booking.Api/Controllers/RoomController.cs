using Hotel.Booking.Api.Configurations;
using Hotel.Booking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Controllers
{
    [ApiController]
    [ApiVersion(ConstantsConfiguration.API_VERSION_1)]
    [Route(ConstantsConfiguration.ROUTE_DEFAULT_CONTROLLER)]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IRoomService _service;

        public RoomController(IRoomService service, ILogger<RoomController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _service.GetAllRoomsActivesAsync();
            if (result.IsSucess)
                return Ok(result.Rooms);

            return NotFound();
        }


        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckRoomAvailabilityAsync([FromQuery] Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var result = await _service.CheckRoomAvailabilityAsync(roomId, checkIn, checkOut);
            if (result.IsSucess)
                return Ok(new
                {
                    result.Status,
                    result.Message
                });

            return NotFound(new
            {
                result.Status,
                result.Message
            });
        }
    }
}