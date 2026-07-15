using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Exceptions;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Results;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Controllers
{
    public class BookingController : ApiController
    {
        private const string UnexpectedErrorMessage = "An unexpected error occurred.";

        private readonly IBookingService _service;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService service, ILogger<BookingController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetAllAsync(cancellationToken);
                if (result.IsSuccess)
                    return ResponseOk(result.Bookings);

                return ResponseFailure(result.StatusResult, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list bookings.");
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
                    return ResponseOk(result.Booking);

                return ResponseFailure(result.StatusResult, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get booking {BookingId}.", id);
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }

        [HttpPost]
        [EnableRateLimiting(ServiceCollection.SensitiveEndpointRateLimitPolicy)]
        public async Task<IActionResult> BookAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _service.BookRoomAsync(request, cancellationToken);
                    if (result.IsSuccess)
                        return ResponseCreated(result.Booking);

                    return ResponseFailure(result.StatusResult, result.Message);
                }
                return BadRequest(ModelState);
            }
            catch (BookingValidationException ex)
            {
                return ResponseFailure(ServiceResultStatus.ValidationError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create booking for room {RoomId}.", request.RoomId);
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }

        [HttpPut]
        [EnableRateLimiting(ServiceCollection.SensitiveEndpointRateLimitPolicy)]
        public async Task<IActionResult> UpdateAsync(UpdateBookingRequest request, CancellationToken cancellationToken)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var result = await _service.UpdateAsync(request, cancellationToken);
                    if (result.IsSuccess)
                        return ResponseOk(result.Booking);

                    return ResponseFailure(result.StatusResult, result.Message);
                }
                return BadRequest(ModelState);
            }
            catch (BookingValidationException ex)
            {
                return ResponseFailure(ServiceResultStatus.ValidationError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update booking {BookingId}.", request.BookingId);
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }

        [HttpPut("cancel/{id}")]
        [EnableRateLimiting(ServiceCollection.SensitiveEndpointRateLimitPolicy)]
        public async Task<IActionResult> CancelAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CancelAsync(id, cancellationToken);
                if (result.IsSuccess)
                    return ResponseOk(result.Message);

                return ResponseFailure(result.StatusResult, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel booking {BookingId}.", id);
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }

        [HttpPut("checkout/{id}")]
        [EnableRateLimiting(ServiceCollection.SensitiveEndpointRateLimitPolicy)]
        public async Task<IActionResult> CheckOutAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CheckOutAsync(id, cancellationToken);
                if (result.IsSuccess)
                    return ResponseOk(result.Message);

                return ResponseFailure(result.StatusResult, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check out booking {BookingId}.", id);
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }

        [HttpPost("check-availability")]
        [EnableRateLimiting(ServiceCollection.SensitiveEndpointRateLimitPolicy)]
        public async Task<IActionResult> CheckRoomAvailabilityAsync([FromBody] AvailabilityRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _service.CheckAvailabilityAsync(request, cancellationToken);
                    if (result.IsSuccess)
                    {
                        var status = result.Status.ToString();
                        return ResponseOk(new
                        {
                            status,
                            result.Message
                        });
                    }

                    return ResponseFailure(result.StatusResult, result.Message);
                }
                return ResponseBadRequest(ModelState);
            }
            catch (BookingValidationException ex)
            {
                return ResponseFailure(ServiceResultStatus.ValidationError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check availability for room {RoomId}.", request.RoomId);
                return ResponseBadRequest(UnexpectedErrorMessage);
            }
        }

        private IActionResult ResponseFailure(ServiceResultStatus statusResult, string message)
        {
            return statusResult switch
            {
                ServiceResultStatus.Conflict => ResponseConflict(message),
                ServiceResultStatus.ValidationError => ResponseBadRequest(message),
                _ => ResponseNotFound(message)
            };
        }
    }
}
