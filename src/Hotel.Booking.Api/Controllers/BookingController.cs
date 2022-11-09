using Microsoft.AspNetCore.Mvc;

namespace Hotel.Booking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;

        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "search")]
        //public IEnumerable<Room> Get([FromQuery]DateTime checkIn, DateTime checkOut)
        //{
        //    var list = new List<Room>() {
        //        new Room            {
        //        Id = Guid.NewGuid(),
        //        Name = "Co-Working - Av. Paulista 1000",
        //        Description = "Sala para com estrutura para realizar, podcats, filmagens e trabalhos.",
        //        IsAvailable = true
        //    }

        //    };
        //    return list;
        //}
    }
}