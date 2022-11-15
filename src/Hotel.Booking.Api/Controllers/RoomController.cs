using Hotel.Booking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Controllers
{
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result.IsSucess)
                    return ResponseOk(result.Room);

                return ResponseNotFound(result.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return ResponseBadRequest(ex.Message);
            }
        }
    }
}