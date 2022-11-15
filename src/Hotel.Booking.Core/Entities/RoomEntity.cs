namespace Hotel.Booking.Core.Entities
{
    public class RoomEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<BookingEntity> Bookings { get; set; }
        
        public RoomEntity(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            IsActive = true;
            Bookings = new List<BookingEntity>();
        }
    }
}
