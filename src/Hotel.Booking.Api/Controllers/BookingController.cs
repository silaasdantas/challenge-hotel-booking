using Hotel.Booking.Api.Configurations;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Models;
using Hotel.Booking.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Hotel.Booking.Api.Controllers
{
    [ApiController]
    [ApiVersion(ConstantsConfiguration.API_VERSION_1)]
    [Route(ConstantsConfiguration.ROUTE_DEFAULT_CONTROLLER)]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;
        private readonly ILogger<BookingController> _logger;
        public readonly BookingContext _dbContext;

        public BookingController(IBookingService service, BookingContext dbContext, ILogger<BookingController> logger)
        {
            _service = service;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await _service.GetAllBookingAsync();
                if (result.IsSucess)
                {
                    return Ok(new
                    {
                        data = new
                        {
                            result.Bookings
                        }
                    });
                }

                return NotFound(new
                {
                    data = new { }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    data = new { },
                    error = ex.Message
                });
            }
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetByIdAsync(Guid bookingId)
        {
            try
            {
                var result = await _service.GetBookingByIdAsync(bookingId);
                if (result.IsSucess)
                {
                    return Ok(new
                    {
                        data = new
                        {
                            result.Booking
                        }
                    });
                }

                return NotFound(new
                {
                    data = result.Booking,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BookRoomAsync(CreateBookingCommand command)
        {
            try
            {
                var result = await _service.BookRoomAsync(command);

                if (result.IsSucess)
                {
                    return Ok(new
                    {
                        data = new
                        {
                            result.Booking
                        }
                    });
                }
                return NotFound(new
                {
                    data = new { },
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    errors = ex.Message
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync(UpdateBookingCommand command)
        {
            try
            {
                var result = await _service.UpdateBookingAsync(command);

                if (result.IsSucess)
                {
                    return Ok(new
                    {
                        data = new
                        {
                            result.Booking
                        }
                    });
                }

                return NotFound(new
                {
                    data = new { },
                    error = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    data = new { },
                    error = ex.Message
                });
            }
        }

        [HttpPut("cancel/{bookingId}")]
        public async Task<IActionResult> CancelAsync(Guid bookingId)
        {
            try
            {
                var result = await _service.CancelAsync(bookingId);
                if (result.IsSucess)
                {
                    return Ok(new
                    {
                        message = "Reserva cancelada com sucesso"
                    });
                }
                return NotFound(new
                {
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    data = new { },
                    error = ex.Message
                });
            }
        }
    }
}