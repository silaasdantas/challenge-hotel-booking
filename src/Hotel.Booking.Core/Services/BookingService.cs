using AutoMapper;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Models;
using Microsoft.Extensions.Logging;

namespace Hotel.Booking.Core.Services
{
    public class BookingService : IBookingService
    {
        private readonly ILogger<RoomService> _logger;
        public readonly IEfRepository<BookingEntity> _repository;
        public readonly IEfRepository<RoomEntity> _repositoryRoom;
        private readonly IMapper _mapper;

        public BookingService(IEfRepository<BookingEntity> repository, IEfRepository<RoomEntity> repositoryRoom, ILogger<RoomService> logger, IMapper mapper)
        {
            _repository = repository;
            _repositoryRoom = repositoryRoom;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<(bool IsSucess, IList<Models.Booking> Bookings, string Message)> GetAllBookingAsync()
        {
            try
            {
                var bookings = await _repository.GetAllAsync();
                if (bookings != null && bookings.Any())
                    return (true, _mapper.Map<IEnumerable<BookingEntity>, IList<Models.Booking>>(bookings), string.Empty);

                return (false, new List<Models.Booking>(), "Not found");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return (false, new List<Models.Booking>(), ex.Message);
            }
        }

        public async Task<(bool IsSucess, Models.Booking Booking, string Message)> GetBookingByIdAsync(Guid bookingId)
        {
            try
            {
                var booking = await _repository.GetByIdAsync(bookingId);
                if (booking != null)
                    return (true, _mapper.Map<BookingEntity, Models.Booking>(booking), string.Empty);

                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSucess, Models.Booking Booking, string Message)> BookRoomAsync(CreateBooking command)
        {
            var roomExist = !await _repository.AnyAsync(_ => _.Id.Equals(command.RoomId));
            if (roomExist)
            {
                var booking = new BookingEntity(command.CheckIn, command.CheckOut, command.RoomId);

                var result = IsValidBooking(booking);
                if (result.IsSucess)
                {
                    var isValid = !await _repository.AnyAsync(_ => 
                       _.RoomId.Equals(command.RoomId) && _.Room.IsActive
                    && _.CheckIn.Date <= command.CheckIn.Date && _.CheckOut.Date >= command.CheckIn.Date
                    || _.CheckIn.Date <= command.CheckOut.Date && _.CheckOut.Date >= command.CheckIn.Date
                    );

                    if (isValid)
                    {
                        await _repository.CreateAsync(booking);

                        var newBooking = await _repository.GetByIdAsync(booking.Id);

                        return (true, _mapper.Map<BookingEntity, Models.Booking>(booking), string.Empty);
                    }

                }
                return (false, null, result.errorMessage);
            }

            return (false, null, "Not found");
        }

        public (bool IsSucess, string errorMessage) IsValidBooking(BookingEntity booking)
        {
            var today = DateTime.Now;

            if (booking.CheckIn.Date <= booking.CreatedAt.Date)
                return (false, "All reservations must start at least the next day of booking.");

            if (booking.CheckOut.Date < booking.CheckIn.Date)
                return (false, "The end date must be greater than the start date.");

            if ((today.AddDays(booking.AdvanceBookingDaysLimit).Date - booking.CheckIn.Date).Days <= -1)
                return (false, $"Rooms can`t be reserved more than {booking.AdvanceBookingDaysLimit} days in advance.");

            if ((booking.CheckOut.Date - booking.CheckIn.Date).Days > booking.StayLimit)
                return (false, $"Rooms can`t be reserved for more than {booking.StayLimit} days.");

            return (true, "");
        }

        public async Task<(bool IsSucess, Models.Booking Booking, string Message)> UpdateBookingAsync(UpdateBooking command)
        {
            var booking = await _repository.GetByIdAsync(command.BookingId);
            if (booking != null)
            {
                booking.Update(command.CheckIn, command.CheckOut);

                var isValid = IsValidBooking(booking);
                if (isValid.IsSucess)
                {
                    var isValidPeriod = !await _repository.AnyAsync(_ =>
                    _.RoomId.Equals(booking.Id)
                    && _.CheckIn.Date <= command.CheckIn.Date && _.CheckOut.Date >= command.CheckIn.Date
                    || _.CheckIn.Date <= command.CheckOut.Date && _.CheckOut.Date >= command.CheckIn.Date
                    );

                    if (isValidPeriod)
                    {
                        await _repository.UpdateAsync(booking);

                        return (true, _mapper.Map<BookingEntity, Models.Booking>(booking), string.Empty);
                    }
                }
                return (false, null, isValid.errorMessage);
            }

            return (true, null, "Not found");
        }

        public async Task<(bool IsSucess, Models.Booking Booking, string Message)> CancelAsync(Guid bookingId)
        {
            var booking = await _repository.GetByIdAsync(bookingId);
            if (booking != null)
            {
                booking.Cancel();

                await _repository.UpdateAsync(booking);

                return (true, _mapper.Map<BookingEntity, Models.Booking>(booking), "Reserva cancelada com sucesso");
            }

            return (false, null, "Not found");
        }
    }
}
