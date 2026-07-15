using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;

namespace Hotel.Booking.Core.Mappers
{
    internal static class ResponseMapper
    {
        public static List<BookingResponse> ToBookingResponseList(IEnumerable<BookingEntity> bookings)
        {
            return bookings.Select(ToBookingResponse).ToList();
        }

        public static BookingResponse ToBookingResponse(BookingEntity booking)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestName = booking.GuestName,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt,
                Room = booking.Room == null ? null! : ToRoomResponse(booking.Room)
            };
        }

        public static List<RoomResponse> ToRoomResponseList(IEnumerable<RoomEntity> rooms)
        {
            return rooms.Select(ToRoomResponse).ToList();
        }

        public static RoomResponse ToRoomResponse(RoomEntity room)
        {
            return new RoomResponse
            {
                Id = room.Id,
                Name = room.Name
            };
        }
    }
}
