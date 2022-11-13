namespace Hotel.Booking.Core.Entities
{
    public class RoomEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public RoomEntity(string name)
        {
            Id = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd"); //Guid.NewGuid();
            Name = name;
            IsActive = true;
        }
    }
}
