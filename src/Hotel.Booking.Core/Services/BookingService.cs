using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Handlers;
using Hotel.Booking.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hotel.Booking.Core.Services
{
    public class BookingService : IBookingService
    {
        public readonly IBookingRespository _bookingRespository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRespository bookingRespository, ILogger<RoomService> logger, IMapper mapper)
        {
            _bookingRespository = bookingRespository;
            _mapper = mapper;
        }
        public async Task<(bool IsSucess, List<BookingResponse> Bookings, string Message)> GetAllBookingAsync()
        {
            try
            {
                var bookings = await _bookingRespository.GetAllAsync();
                if (bookings != null && bookings.Any())
                    return (true, _mapper.Map<IEnumerable<BookingEntity>, List<BookingResponse>>(bookings), string.Empty);

                return (false, new List<BookingResponse>(), "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, BookingResponse Booking, string Message)> GetBookingByIdAsync(Guid bookingId)
        {
            try
            {
                var booking = await _bookingRespository.GetByIdAsync(bookingId);
                if (booking != null)
                    return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);

                return (false, null, "Not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool IsSucess, BookingResponse Booking, string Message)> BookRoomAsync(BookingRequest request)
        {
            try
            {
                var exists = await _bookingRespository.AnyAsync(_ => _.RoomId.Equals(request.RoomId));
                if (exists)
                {
                    ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

                    var result = await _bookingRespository.CheckRoomAvailabilityAsync(request.RoomId, request.CheckIn, request.CheckOut);
                    if (result.Equals(RoomStatusValueObject.Available))
                    {
                        var booking = new BookingEntity(request.CheckIn, request.CheckOut, request.RoomId);
                        await _bookingRespository.CreateAsync(booking);

                        return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);
                    }

                    return (false, null, "Room not available for booking on this date.");
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task<(bool IsSucess, BookingResponse Booking, string Message)> UpdateBookingAsync(UpdateBookingRequest request)
        {
            try
            {
                var booking = await _bookingRespository.GetByIdAsync(request.BookingId);
                if (booking != null)
                {
                    ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

                    var result = await _bookingRespository.CheckRoomAvailabilityAsync(booking.RoomId, request.CheckIn, request.CheckOut);
                    if (result.Equals(RoomStatusValueObject.Available))
                    {
                        booking.Update(request.CheckIn, request.CheckOut);
                        await _bookingRespository.UpdateAsync(booking);

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

        public async Task<(bool IsSucess, BookingResponse Booking, string Message)> CancelAsync(Guid bookingId)
        {
            try
            {
                var booking = await _bookingRespository.GetByIdAsync(bookingId);
                if (booking != null)
                {
                    booking.Cancel();
                    await _bookingRespository.UpdateAsync(booking);

                    return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), "Booking successfully canceled");
                }

                return (false, null, "Not found");
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
