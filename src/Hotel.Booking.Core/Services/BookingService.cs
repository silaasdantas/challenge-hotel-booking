using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Handlers;
using Hotel.Booking.Core.Interfaces;

namespace Hotel.Booking.Core.Services
{
    public class BookingService : IBookingService
    {
        public readonly IBookingRespository _respository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRespository respository, IMapper mapper)
        {
            _respository = respository;
            _mapper = mapper;
        }

        public async Task<(bool IsSucess, List<BookingResponse> Bookings, string Message)> GetAllAsync()
        {
            try
            {
                var bookings = await _respository.GetAllAsync();
                if (bookings != null && bookings.Any())
                    return (true, _mapper.Map<IEnumerable<BookingEntity>, List<BookingResponse>>(bookings), string.Empty);

                return (false, new List<BookingResponse>(), "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> GetByIdAsync(Guid bookingId)
        {
            try
            {
                var booking = await _respository.GetByIdAsync(bookingId);
                if (booking != null)
                    return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);

                return (false, null, Message: "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request)
        {
            try
            {
                var checkAvailabilityResult = await CheckAvailabilityAsync(request);

                if (checkAvailabilityResult.IsSucess)
                {
                    ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

                    if (checkAvailabilityResult.Status.Equals(RoomStatusValueObject.Available))
                    {
                        var booking = new BookingEntity(request.CheckIn, request.CheckOut, request.RoomId);
                        await _respository.CreateAsync(booking);

                        return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);
                    }

                    return (false, null, "Room not available for booking on this date");
                }
                return (false, null, checkAvailabilityResult.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> UpdateAsync(UpdateBookingRequest request)
        {
            try
            {
                var booking = await _respository.GetByIdAsync(request.BookingId);
                if (booking != null)
                {
                    ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

                    var result = await _respository.CheckRoomAvailabilityAsync(booking.RoomId, request.CheckIn, request.CheckOut);
                    if (result.Equals(RoomStatusValueObject.Available))
                    {
                        booking.Update(request.CheckIn, request.CheckOut);
                        await _respository.UpdateAsync(booking);

                        return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);
                    }
                    return (false, null, "Room not available for booking on this date.");
                }

                return (false, null, "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId)
        {
            try
            {
                var booking = await _respository.GetByIdAsync(bookingId);
                if (booking != null)
                {
                    booking.Cancel();
                    await _respository.UpdateAsync(booking);

                    return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), "Booking successfully canceled");
                }

                return (false, null, "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, RoomStatusValueObject Status, string Message)> CheckAvailabilityAsync(BookingRequest request)
        {
            try
            {
                var roomExists = await _respository.AnyAsync(_ => _.RoomId.Equals(request.RoomId));
                if (roomExists)
                {
                    ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

                    var result = await _respository.CheckRoomAvailabilityAsync(request.RoomId, request.CheckIn, request.CheckOut);

                    if (result.Equals(RoomStatusValueObject.Available))
                        return (true, RoomStatusValueObject.Available, "Room available to book");
                    else
                        return (true, RoomStatusValueObject.Booked, "Room not available for booking on this date");
                }
                return (false, RoomStatusValueObject.None, "Room not found");
            }
            catch (Exception)
            {
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
