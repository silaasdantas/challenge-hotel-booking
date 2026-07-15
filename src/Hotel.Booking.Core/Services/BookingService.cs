using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Exceptions;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Mappers;
using Hotel.Booking.Core.Results;
using Hotel.Booking.Core.Validators;

namespace Hotel.Booking.Core.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;

        public BookingService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool IsSuccess, ServiceResultStatus StatusResult, List<BookingResponse> Bookings, string Message)> GetAllAsync(CancellationToken cancellationToken)
        {
            var bookings = await _repository.GetAllAsync(cancellationToken);
            if (bookings != null && bookings.Any())
                return (true, ServiceResultStatus.Success, ResponseMapper.ToBookingResponseList(bookings), string.Empty);

            return (false, ServiceResultStatus.NotFound, new List<BookingResponse>(), "Not found");
        }

        public async Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            var booking = await _repository.GetByIdAsync(bookingId, cancellationToken);
            if (booking != null)
                return (true, ServiceResultStatus.Success, ResponseMapper.ToBookingResponse(booking), string.Empty);

            return (false, ServiceResultStatus.NotFound, null, Message: "Not found");
        }

        public async Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> BookRoomAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            BookingRequestValidator.ValidateForBooking(request);

            var roomExists = await _repository.AnyAsync(_ => _.RoomId.Equals(request.RoomId), cancellationToken);
            if (!roomExists)
                return (false, ServiceResultStatus.NotFound, null, "Room not found");

            var booking = new BookingEntity(request.CheckIn, request.CheckOut, request.RoomId, request.GuestName);
            var createResult = await _repository.TryCreateBookingAsync(booking, cancellationToken);
            if (!createResult.IsSuccess)
                return (false, ServiceResultStatus.Conflict, null, "Room not available for booking on this date");

            return (true, ServiceResultStatus.Success, ResponseMapper.ToBookingResponse(createResult.Booking), string.Empty);
        }

        public async Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> UpdateAsync(UpdateBookingRequest request, CancellationToken cancellationToken)
        {
            BookingRequestValidator.ValidateForUpdate(request);

            BookingDateValidator.Validate(request.CheckIn, request.CheckOut);

            var updateResult = await _repository.TryUpdateBookingDatesAsync(request.BookingId, request.CheckIn, request.CheckOut, cancellationToken);
            if (updateResult.NotFound)
                return (false, ServiceResultStatus.NotFound, null, "Not found");

            if (!updateResult.IsSuccess || updateResult.Booking == null)
                return (false, ServiceResultStatus.Conflict, null, "Room not available for booking on this date.");

            return (true, ServiceResultStatus.Success, ResponseMapper.ToBookingResponse(updateResult.Booking), string.Empty);
        }

        public async Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> CancelAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            var booking = await _repository.GetByIdAsync(bookingId, cancellationToken);
            if (booking == null)
                return (false, ServiceResultStatus.NotFound, null, "Not found");

            booking.Cancel();
            await _repository.UpdateAsync(booking, cancellationToken);

            return (true, ServiceResultStatus.Success, ResponseMapper.ToBookingResponse(booking), "Booking successfully canceled");
        }

        public async Task<(bool IsSuccess, ServiceResultStatus StatusResult, BookingResponse? Booking, string Message)> CheckOutAsync(Guid bookingId, CancellationToken cancellationToken)
        {
            var booking = await _repository.GetByIdAsync(bookingId, cancellationToken);
            if (booking == null)
                return (false, ServiceResultStatus.NotFound, null, "Not found");

            try
            {
                booking.CheckOutRoom();
            }
            catch (BookingValidationException ex)
            {
                return (false, ServiceResultStatus.ValidationError, null, ex.Message);
            }

            await _repository.UpdateAsync(booking, cancellationToken);

            return (true, ServiceResultStatus.Success, ResponseMapper.ToBookingResponse(booking), "Booking successfully checked out");
        }

        public async Task<(bool IsSuccess, ServiceResultStatus StatusResult, RoomStatusValueObject Status, string Message)> CheckAvailabilityAsync(AvailabilityRequest request, CancellationToken cancellationToken)
        {
            BookingRequestValidator.ValidateForAvailability(request);

            var roomExists = await _repository.AnyAsync(_ => _.RoomId.Equals(request.RoomId), cancellationToken);
            if (!roomExists)
                return (false, ServiceResultStatus.NotFound, RoomStatusValueObject.None, "Room not found");

            BookingDateValidator.Validate(request.CheckIn, request.CheckOut);

            var result = await _repository.CheckRoomAvailabilityAsync(request.RoomId, request.CheckIn, request.CheckOut, null, cancellationToken);
            if (result.Equals(RoomStatusValueObject.Available))
                return (true, ServiceResultStatus.Success, RoomStatusValueObject.Available, "Room available to book");

            return (false, ServiceResultStatus.Conflict, RoomStatusValueObject.Booked, "Room not available for booking on this date");
        }

    }
}
