namespace Hotel.Booking.Core.Interfaces
{
    public interface IHandler
    {
        void Handle(DateTime checkIn, DateTime checkOut);
        IHandler SetNext(IHandler next);
    }
}
