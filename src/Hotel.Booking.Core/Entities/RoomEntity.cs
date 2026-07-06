namespace Hotel.Booking.Core.Entities
{
    public class RoomEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public ICollection<BookingEntity> Bookings { get; private set; }
        
        public RoomEntity(string name)
            : this(Guid.NewGuid(), name)
        {
        }

        public RoomEntity(Guid id, string name)
        {
            Id = id;
            Name = name;
            IsActive = true;
            Bookings = new List<BookingEntity>();
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
