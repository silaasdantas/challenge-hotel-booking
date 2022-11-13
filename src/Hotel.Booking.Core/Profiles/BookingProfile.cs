using AutoMapper;
using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.Profiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<BookingEntity, Models.Booking>();
            CreateMap<Models.Booking, BookingEntity>();
        }
    }
}
