using AutoMapper;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Profiles;
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

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new RoomProfile()));
            service = new RoomService(repositoryMock.Object, new Mapper(configuration));
        }

        [Fact]
        public async Task MustReturnAListRooms_GetAllRoomsActives()
        {
            repositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<RoomEntity> { CreateRoom() });

            var result = await service.GetAllRoomsActivesAsync();

            result.IsSuccess.ShouldBeTrue();
            result.Rooms.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAListRoomsEmpty_GetAllRoomsActives()
        {
            repositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<RoomEntity>());

            var result = await service.GetAllRoomsActivesAsync();

            result.IsSuccess.ShouldBeFalse();
            result.Rooms.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneRoomById_GetByIdAsync()
        {
            repositoryMock.Setup(_ => _.GetByIdAsync(RoomId))
                .ReturnsAsync(CreateRoom());

            var result = await service.GetByIdAsync(RoomId);

            result.IsSuccess.ShouldBeTrue();
            result.Room.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnNotFound_WhenRoomDoesNotExist_GetByIdAsync()
        {
            var roomId = Guid.NewGuid();
            repositoryMock.Setup(_ => _.GetByIdAsync(roomId))
                .ReturnsAsync((RoomEntity)null!);

            var result = await service.GetByIdAsync(roomId);

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
