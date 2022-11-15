using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.Profiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<BookingEntity, BookingResponse>()
                 .BeforeMap((src, dest) => dest.Status = src.Status.ToString());

            CreateMap<BookingResponse, BookingEntity>();
        }
    }
}
