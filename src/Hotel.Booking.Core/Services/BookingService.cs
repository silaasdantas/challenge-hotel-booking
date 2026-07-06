using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Results;
using Hotel.Booking.Core.Validators;

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

        public async Task<(bool IsSucess, ServiceResultStatus StatusResult, List<BookingResponse> Bookings, string Message)> GetAllAsync()
        {
            var bookings = await _respository.GetAllAsync();
            if (bookings != null && bookings.Any())
                return (true, ServiceResultStatus.Success, _mapper.Map<IEnumerable<BookingEntity>, List<BookingResponse>>(bookings), string.Empty);

            return (false, ServiceResultStatus.NotFound, new List<BookingResponse>(), "Not found");
        }

        public async Task<(bool IsSucess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> GetByIdAsync(Guid bookingId)
        {
            var booking = await _respository.GetByIdAsync(bookingId);
            if (booking != null)
                return (true, ServiceResultStatus.Success, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);

            return (false, ServiceResultStatus.NotFound, null, Message: "Not found");
        }

        public async Task<(bool IsSucess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request)
        {
            var checkAvailabilityResult = await CheckAvailabilityAsync(request);
            if (!checkAvailabilityResult.IsSucess)
                return (false, checkAvailabilityResult.StatusResult, null, checkAvailabilityResult.Message);

            if (!checkAvailabilityResult.Status.Equals(RoomStatusValueObject.Available))
                return (false, ServiceResultStatus.Conflict, null, "Room not available for booking on this date");

            var booking = new BookingEntity(request.CheckIn, request.CheckOut, request.RoomId, request.GuestName);
            await _respository.CreateAsync(booking);

            return (true, ServiceResultStatus.Success, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);
        }

        public async Task<(bool IsSucess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> UpdateAsync(UpdateBookingRequest request)
        {
            var booking = await _respository.GetByIdAsync(request.BookingId);
            if (booking == null)
                return (false, ServiceResultStatus.NotFound, null, "Not found");

            BookingDateValidator.Validate(request.CheckIn, request.CheckOut);

            var result = await _respository.CheckRoomAvailabilityAsync(booking.RoomId, request.CheckIn, request.CheckOut);
            if (!result.Equals(RoomStatusValueObject.Available))
                return (false, ServiceResultStatus.Conflict, null, "Room not available for booking on this date.");

            booking.Update(request.CheckIn, request.CheckOut);
            await _respository.UpdateAsync(booking);

            return (true, ServiceResultStatus.Success, _mapper.Map<BookingEntity, BookingResponse>(booking), string.Empty);
        }

        public async Task<(bool IsSucess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId)
        {
            var booking = await _respository.GetByIdAsync(bookingId);
            if (booking == null)
                return (false, ServiceResultStatus.NotFound, null, "Not found");

            booking.Cancel();
            await _respository.UpdateAsync(booking);

            return (true, ServiceResultStatus.Success, _mapper.Map<BookingEntity, BookingResponse>(booking), "Booking successfully canceled");
        }

        public async Task<(bool IsSucess, ServiceResultStatus StatusResult, RoomStatusValueObject Status, string Message)> CheckAvailabilityAsync(BookingRequest request)
        {
            var roomExists = await _respository.AnyAsync(_ => _.RoomId.Equals(request.RoomId));
            if (!roomExists)
                return (false, ServiceResultStatus.NotFound, RoomStatusValueObject.None, "Room not found");

            BookingDateValidator.Validate(request.CheckIn, request.CheckOut);

            var result = await _respository.CheckRoomAvailabilityAsync(request.RoomId, request.CheckIn, request.CheckOut);
            if (result.Equals(RoomStatusValueObject.Available))
                return (true, ServiceResultStatus.Success, RoomStatusValueObject.Available, "Room available to book");

            return (false, ServiceResultStatus.Conflict, RoomStatusValueObject.Booked, "Room not available for booking on this date");
        }

    }
}
