using Hotel.Booking.Api.Configurations;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Controllers
{
    [ApiVersion(ConstantsConfiguration.API_VERSION_1)]
    [Route(ConstantsConfiguration.ROUTE_DEFAULT_CONTROLLER)]
    public class RoomController : ApiController
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IRoomService _service;

        public RoomController(IRoomService service, ILogger<RoomController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await _service.GetAllRoomsActivesAsync();
                if (result.IsSucess)
                    return ResponseOk(result.Rooms);

                return ResponseNotFound();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return ResponseBadRequest(ex.Message);
            }
        }


        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckRoomAvailabilityAsync([FromBody] BookingRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _service.CheckRoomAvailabilityAsync(request);
                    if (result.IsSucess)
                    {
                        return ResponseOk(new
                        {
                            result.Status,
                            result.Message
                        });
                    }

                    return ResponseNotFound(result.Message);
                }
                return ResponseBadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return ResponseBadRequest(ex.Message);
            }
        }
    }
}