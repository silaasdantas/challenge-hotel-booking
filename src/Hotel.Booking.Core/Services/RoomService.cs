﻿using AutoMapper;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Handlers;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace Hotel.Booking.Core.Services
{
    public class RoomService : IRoomService
    {
        private readonly ILogger<RoomService> _logger;
        public readonly IRoomRespository _roomRespository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRespository roomRespository,
            ILogger<RoomService> logger,
            IMapper mapper)
        {
            _roomRespository = roomRespository;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<(bool IsSucess, IList<RoomResponse> Rooms, string Message)>
            GetAllRoomsActivesAsync()
        {
            try
            {
                var rooms = await _roomRespository.GetAllAsync();
                if (rooms != null && rooms.Any())
                {
                    var result = _mapper.Map<IList<RoomEntity>, IList<RoomResponse>>(rooms);
                    return (true, result, string.Empty);
                }
                return (false, new List<RoomResponse>(), "Not found");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<(bool IsSucess, string Status, string Message)>
            CheckRoomAvailabilityAsync(BookingRequest request)
        {
            try
            {
                var room = await _roomRespository.GetByIdAsync(request.RoomId);
                if (room == null)
                    return (false, string.Empty, "Room not found");

                ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

                var result = await _roomRespository.CheckRoomAvailabilityAsync(request.RoomId, request.CheckIn, request.CheckOut);

                if (result.Equals(RoomStatusValueObject.Available))
                    return (true, RoomStatusValueObject.Available.ToString(), "Room available to book.");
                else
                    return (true, RoomStatusValueObject.Booked.ToString(), "Room not available for booking on this date.");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
        }

        private static void ValidDateCheckInAndCheckout(DateTime checkIn, DateTime checkOut)
        {
            var handler = new NextDayOfBookingValidationHandler();
            handler.SetNext(new DateGreaterThanStartValidationHandler())
                .SetNext(new AdvanceBookingDaysLimitValidationHandler())
                .SetNext(new StayLimitValidationHandler());

            handler.Handle(checkIn, checkOut);
        }
    }
}