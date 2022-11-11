using Hotel.Booking.Core.Models;
using Hotel.Booking.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Booking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        public readonly BookingContext _dbContext;

        public RoomController(BookingContext dbContext, ILogger<RoomController> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            SeedData();
        }


        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _dbContext.Rooms.AsNoTracking().Where(_ => _.IsActive).ToListAsync();
            if (result != null)
                return Ok(result);

            return NotFound();
        }


        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckRoomAvailabilityAsync([FromQuery] Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var result = !await _dbContext.Bookings.Include(_ => _.Room)
                .AnyAsync(_ => _.RoomId.Equals(roomId)
                && _.CheckIn.Date <= checkIn.Date && _.CheckOut.Date >= checkIn.Date
                || _.CheckIn.Date <= checkOut.Date && _.CheckOut.Date >= checkIn.Date
                );

            return Ok(new { availability = result });
        }    

        private void SeedData()
        {
            if (!_dbContext.Rooms.Any())
            {
                var roomSeed = new RoomEntity("Premium Suite, Lagoon View", true);
                _dbContext.Rooms.Add(roomSeed);
                _dbContext.Bookings.Add(new BookingEntity(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), roomSeed.Id));
                _dbContext.Bookings.Add(new BookingEntity(DateTime.Now.AddDays(10), DateTime.Now.AddDays(12), roomSeed.Id));
                _dbContext.SaveChanges();
            }
        }
    }
}