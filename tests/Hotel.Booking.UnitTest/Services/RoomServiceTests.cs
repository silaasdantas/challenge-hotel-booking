using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Moq;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> repositoryMock;
        private readonly RoomService service;

        private static readonly Guid RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd");

        public RoomServiceTests()
        {
            repositoryMock = new Mock<IRoomRepository>();
            service = new RoomService(repositoryMock.Object);
        }

        [Fact]
        public async Task MustReturnAListRooms_GetAllRoomsActives()
        {
            repositoryMock.Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RoomEntity> { CreateRoom() });

            var result = await service.GetAllRoomsActivesAsync(CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Rooms.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAListRoomsEmpty_GetAllRoomsActives()
        {
            repositoryMock.Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RoomEntity>());

            var result = await service.GetAllRoomsActivesAsync(CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.Rooms.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneRoomById_GetByIdAsync()
        {
            repositoryMock.Setup(_ => _.GetByIdAsync(RoomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateRoom());

            var result = await service.GetByIdAsync(RoomId, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Room.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnNotFound_WhenRoomDoesNotExist_GetByIdAsync()
        {
            var roomId = Guid.NewGuid();
            repositoryMock.Setup(_ => _.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RoomEntity?)null);

            var result = await service.GetByIdAsync(roomId, CancellationToken.None);

            result.IsSuccess.ShouldBeFalse();
            result.Room.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        private static RoomEntity CreateRoom()
        {
            return new RoomEntity(RoomId, "MIO Cancun Hotel Boutique, Queen Suite");
        }
    }
}
