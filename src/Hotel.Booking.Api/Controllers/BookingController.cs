using Hotel.Booking.Core.Models;
using Hotel.Booking.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        public readonly BookingContext _dbContext;

        public BookingController(BookingContext dbContext, ILogger<BookingController> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            SeedData();
        }


        [HttpGet("rooms")]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _dbContext.Rooms.AsNoTracking().Where(_ => _.IsActive).ToListAsync();
            if (result != null)
                return Ok(result);

            return NotFound();
        }


        [HttpGet("check-room-availability")]
        public async Task<IActionResult> CheckRoomAvailabilityAsync([FromQuery] Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var result = !await _dbContext.Bookings.Include(_ => _.Room)
                .AnyAsync(_ => _.RoomId.Equals(roomId)
                && _.CheckIn.Date <= checkIn.Date && _.CheckOut.Date >= checkIn.Date
                || _.CheckIn.Date <= checkOut.Date && _.CheckOut.Date >= checkIn.Date
                );

            return Ok(new { availability = result });
        }

        [HttpGet()]
        public async Task<IActionResult> GetBookingAsync(Guid bookingId)
        {
            try
            {
                var booking = await _dbContext.Bookings.AsNoTracking().FirstOrDefaultAsync(_ => _.Id.Equals(bookingId));
                if (booking != null)
                {
                    return Ok(new
                    {
                        data = new
                        {
                            booking
                        }
                    });
                }

                return NotFound(new
                {
                    data = new { },
                    error = $"Not Found - Booking {bookingId}"
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

        [HttpPost]
        public async Task<IActionResult> BookingAsync(CreateBookingCommand command)
        {
            var roomExists = await _dbContext.Rooms.AsNoTracking().AnyAsync(_ => _.Id.Equals(command.RoomId));
            if (!roomExists)
            {
                return NotFound(new
                {
                    data = new { },
                    error = $"Room {command.RoomId} Not Found"
                });
            }


            var booking = new BookingEntity(command.CheckIn, command.CheckOut, command.RoomId);

            var resultValid = booking.IsValidBooking(command.CheckIn, command.CheckOut);
            var result = !await _dbContext.Bookings.Include(_ => _.Room)
                .AnyAsync(_ => _.RoomId.Equals(command.RoomId)
                && _.CheckIn.Date <= command.CheckIn.Date && _.CheckOut.Date >= command.CheckIn.Date
                || _.CheckIn.Date <= command.CheckOut.Date && _.CheckOut.Date >= command.CheckIn.Date
                );

            if (resultValid.IsSucess && result)
            {
                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync();

                var newBooking = await _dbContext.Bookings.Include(_ => _.Room).FirstOrDefaultAsync(_ => _.Id.Equals(booking.Id));

                return Ok(new
                {
                    data = new
                    {
                        booking = newBooking
                    }
                });
            }

            return BadRequest(new
            {
                data = new { },
                error = resultValid.errorMessage
            });

        }

        [HttpPut("update")]
        public async Task<IActionResult> CancelBookingAsync(UpdateBookingCommand command)
        {
            try
            {
                var booking = await _dbContext.Bookings.AsNoTracking().FirstOrDefaultAsync(_ => _.Id.Equals(command.BookingId));
                if (booking != null)
                {
                    var result = booking.Update(command.CheckIn, command.CheckOut);
                    if (result.IsSucess)
                    {
                        var entity = _dbContext.Bookings.Attach(booking);
                        entity.State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();

                        return Ok(new
                        {
                            data = new
                            {
                                booking
                            }
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            data = new { },
                            error = result.errorMessage
                        });
                    }

                }

                return NotFound(new
                {
                    data = new { },
                    error = $"Booking {command.BookingId} Not Found"
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

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelBookingAsync(Guid bookingId)
        {
            try
            {
                var booking = await _dbContext.Bookings.AsNoTracking().FirstOrDefaultAsync(_ => _.Id.Equals(bookingId));
                if (booking != null)
                {
                    booking.Cancel();
                    var entity = _dbContext.Bookings.Attach(booking);
                    entity.State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    return Ok(new
                    {
                        data = new
                        {
                            booking
                        }
                    });
                }

                return NotFound(new
                {
                    data = new { },
                    error = $"Not Found - Booking {bookingId}"
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

        private void SeedData()
        {
            if (!_dbContext.Rooms.Any())
            {
                var roomSeed = new RoomEntity("Premium Suite, Lagoon View", true);
                _dbContext.Rooms.Add(roomSeed);
                //_dbContext.Bookings.Add(new BookingEntity(DateTime.Now, DateTime.Now.AddDays(3), roomSeed.Id));
                //_dbContext.Bookings.Add(new BookingEntity(DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomSeed.Id));
                _dbContext.SaveChanges();
            }
        }
    }
}