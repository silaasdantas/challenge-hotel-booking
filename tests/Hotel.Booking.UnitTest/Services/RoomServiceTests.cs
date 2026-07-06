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
        private readonly Mock<IRoomRespository> respositoryMock;
        private readonly RoomService service;

        private static readonly Guid RoomId = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd");

        public RoomServiceTests()
        {
            respositoryMock = new Mock<IRoomRespository>();

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new RoomProfile()));
            service = new RoomService(respositoryMock.Object, new Mapper(configuration));
        }

        [Fact]
        public async Task MustReturnAListRooms_GetAllRoomsActives()
        {
            respositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<RoomEntity> { CreateRoom() });

            var result = await service.GetAllRoomsActivesAsync();

            result.IsSucess.ShouldBeTrue();
            result.Rooms.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAListRoomsEmpty_GetAllRoomsActives()
        {
            respositoryMock.Setup(_ => _.GetAllAsync())
                .ReturnsAsync(new List<RoomEntity>());

            var result = await service.GetAllRoomsActivesAsync();

            result.IsSucess.ShouldBeFalse();
            result.Rooms.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneRoomById_GetByIdAsync()
        {
            respositoryMock.Setup(_ => _.GetByIdAsync(RoomId))
                .ReturnsAsync(CreateRoom());

            var result = await service.GetByIdAsync(RoomId);

            result.IsSucess.ShouldBeTrue();
            result.Room.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnNotFound_WhenRoomDoesNotExist_GetByIdAsync()
        {
            var roomId = Guid.NewGuid();
            respositoryMock.Setup(_ => _.GetByIdAsync(roomId))
                .ReturnsAsync((RoomEntity)null!);

            var result = await service.GetByIdAsync(roomId);

            result.IsSucess.ShouldBeFalse();
            result.Room.ShouldBeNull();
            result.Message.ShouldBe("Not found");
        }

        private static RoomEntity CreateRoom()
        {
            return new RoomEntity(RoomId, "MIO Cancun Hotel Boutique, Queen Suite");
        }
    }
}
