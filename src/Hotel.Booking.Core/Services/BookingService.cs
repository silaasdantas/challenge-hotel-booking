using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Handlers;
using Hotel.Booking.Core.Interfaces;

namespace Hotel.Booking.Core.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRespository _respository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRespository respository, IMapper mapper)
        {
            _respository = respository;
            _mapper = mapper;
        }

        public async Task<(bool IsSucess, List<BookingResponse> Bookings, string Message)> GetAllAsync()
        {
            var bookings = await _respository.GetAllAsync();
            if (bookings != null && bookings.Any())
                return (true, _mapper.Map<IEnumerable<BookingEntity>, List<BookingResponse>>(bookings), string.Empty);

            return (false, new List<BookingResponse>(), "Not found");
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> GetByIdAsync(Guid bookingId)
        {
            var booking = await _respository.GetByIdAsync(bookingId);
            if (booking != null)
                return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);

            return (false, null, Message: "Not found");
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request)
        {
            var checkAvailabilityResult = await CheckAvailabilityAsync(request);
            if (!checkAvailabilityResult.IsSucess)
                return (false, null, checkAvailabilityResult.Message);

            if (!checkAvailabilityResult.Status.Equals(RoomStatusValueObject.Available))
                return (false, null, "Room not available for booking on this date");

            var booking = new BookingEntity(request.CheckIn, request.CheckOut, request.RoomId, request.GuestName);
            await _respository.CreateAsync(booking);

            return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> UpdateAsync(UpdateBookingRequest request)
        {
            var booking = await _respository.GetByIdAsync(request.BookingId);
            if (booking == null)
                return (false, null, "Not found");

            ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

            var result = await _respository.CheckRoomAvailabilityAsync(booking.RoomId, request.CheckIn, request.CheckOut);
            if (!result.Equals(RoomStatusValueObject.Available))
                return (false, null, "Room not available for booking on this date.");

            booking.Update(request.CheckIn, request.CheckOut);
            await _respository.UpdateAsync(booking);

            return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);
        }

        public async Task<(bool IsSucess, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId)
        {
            var booking = await _respository.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, null, "Not found");

            booking.Cancel();
            await _respository.UpdateAsync(booking);

            return (true, _mapper.Map<BookingEntity, BookingResponse>(booking), "Booking successfully canceled");
        }

        public async Task<(bool IsSucess, RoomStatusValueObject Status, string Message)> CheckAvailabilityAsync(BookingRequest request)
        {
            var roomExists = await _respository.AnyAsync(_ => _.RoomId.Equals(request.RoomId));
            if (!roomExists)
                return (false, RoomStatusValueObject.None, "Room not found");

            ValidDateCheckInAndCheckout(request.CheckIn, request.CheckOut);

            var result = await _respository.CheckRoomAvailabilityAsync(request.RoomId, request.CheckIn, request.CheckOut);
            if (result.Equals(RoomStatusValueObject.Available))
                return (true, RoomStatusValueObject.Available, "Room available to book");

            return (false, RoomStatusValueObject.Booked, "Room not available for booking on this date");
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
