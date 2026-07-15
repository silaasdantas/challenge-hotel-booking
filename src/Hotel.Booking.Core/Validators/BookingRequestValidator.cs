using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Exceptions;

namespace Hotel.Booking.Core.Validators
{
    public static class BookingRequestValidator
    {
        public static void ValidateForBooking(BookingRequest request)
        {
            ValidateForAvailability(request);

            if (string.IsNullOrWhiteSpace(request.GuestName))
                throw new BookingValidationException("GuestName is required.");
        }

        public static void ValidateForAvailability(BookingRequest request)
        {
            ValidateForAvailability(request.RoomId, request.CheckIn, request.CheckOut);
        }

        public static void ValidateForAvailability(AvailabilityRequest request)
        {
            ValidateForAvailability(request.RoomId, request.CheckIn, request.CheckOut);
        }

        public static void ValidateForUpdate(UpdateBookingRequest request)
        {
            if (request.BookingId == Guid.Empty)
                throw new BookingValidationException("BookingId is required.");

            ValidateDates(request.CheckIn, request.CheckOut);
        }

        private static void ValidateForAvailability(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            if (roomId == Guid.Empty)
                throw new BookingValidationException("RoomId is required.");

            ValidateDates(checkIn, checkOut);
        }

        private static void ValidateDates(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn == default)
                throw new BookingValidationException("CheckIn is required.");

            if (checkOut == default)
                throw new BookingValidationException("CheckOut is required.");
        }
    }
}
