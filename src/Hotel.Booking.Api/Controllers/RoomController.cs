using Hotel.Booking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Controllers
{
    public class RoomController : ApiController
    {
        private const string UnexpectedErrorMessage = "An unexpected error occurred.";

        private readonly ILogger<RoomController> _logger;
        private readonly IRoomService _service;

        public RoomController(IRoomService service, ILogger<RoomController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetAllRoomsActivesAsync(cancellationToken);
                if (result.IsSuccess)
                    return ResponseOk(result.Rooms);

                return ResponseNotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list active rooms.");
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                    return ResponseOk(result.Room);

                return ResponseNotFound(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get room {RoomId}.", id);
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }
    }
}
