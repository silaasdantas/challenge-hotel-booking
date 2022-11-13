using AutoMapper;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Models;

namespace Hotel.Booking.Core.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomEntity, Room>();
            CreateMap<Room, RoomEntity>();
        }
    }
}
