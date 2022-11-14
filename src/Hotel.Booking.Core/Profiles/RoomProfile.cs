using AutoMapper;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.DTOs;

namespace Hotel.Booking.Core.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomEntity, RoomResponse>();
            CreateMap<RoomResponse, RoomEntity>();
        }
    }
}
