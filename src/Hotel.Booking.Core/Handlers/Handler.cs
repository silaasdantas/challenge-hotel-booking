using Hotel.Booking.Core.Interfaces;

namespace Hotel.Booking.Core.Handlers
{
    public abstract class Handler
    {
        private IHandler Next { get; set; }

        public virtual void Handle(DateTime checkIn, DateTime checkOut)
        {
            Next?.Handle(checkIn, checkOut);
        }

        public IHandler SetNext(IHandler next)
        {
            Next = next;
            return Next;
        }
    }
}
