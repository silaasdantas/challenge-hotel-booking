using Hotel.Booking.Api.Configurations;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Controllers
{

    [ApiVersion(ConstantsConfiguration.API_VERSION_1)]
    [Route(ConstantsConfiguration.ROUTE_DEFAULT_CONTROLLER)]
    public class BookingController : ApiController
    {
        private readonly IBookingService _service;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService service, ILogger<BookingController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await _service.GetAllAsync();
                if (result.IsSucess)
                    return ResponseOk(result.Bookings);

                return ResponseNotFound(result.Message);
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
                    return ResponseOk(result.Booking);

                return ResponseNotFound(result.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return ResponseBadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> BookAsync(BookingRequest request)
        {
            try
            {
                var result = await _service.BookRoomAsync(request);
                if (result.IsSucess)
                    return ResponseCreated(result.Booking);

                return ResponseNotFound(result.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return ResponseBadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateBookingRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(request);
                if (result.IsSucess)
                    return ResponseOk(result.Booking);

                return ResponseNotFound(result.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return ResponseBadRequest(ex.Message);
            }
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAsync(Guid id)
        {
            try
            {
                var result = await _service.CancelAsync(id);
                if (result.IsSucess)
                    return ResponseOk(result.Message);

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