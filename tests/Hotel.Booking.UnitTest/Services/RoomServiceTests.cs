using AutoMapper;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Profiles;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class RoomServiceTests
    {
        private DbContextOptions<HotelDbContext> options;
        private HotelDbContext dbContext;
        private RoomRespository respository;
        private RoomProfile roomProfile;
        private RoomService service;

        public RoomServiceTests()
        {
            options = new DbContextOptionsBuilder<HotelDbContext>()
              .UseInMemoryDatabase("BookingTestingDB")
              .Options;
            dbContext = new HotelDbContext(options);
            respository = new RoomRespository(dbContext);
            roomProfile = new RoomProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(roomProfile));
            var mapper = new Mapper(configuration);
            service = new RoomService(respository, mapper);
        }

        [Fact]
        public async Task MustReturnAListRooms_GetAllRoomsActives()
        {
            //act
            var result = await service.GetAllRoomsActivesAsync();

            //assert
            result.IsSucess.ShouldBe(true);
            result.Rooms.ShouldNotBeEmpty();
            result.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAListRoomsEmpty_GetAllRoomsActives()
        {
            var serviceMock = new Mock<IRoomService>();
            serviceMock.Setup(_ => _.GetAllRoomsActivesAsync()).Returns(GetAllRoomsActivesMock);

            //act
            var result = await serviceMock.Object.GetAllRoomsActivesAsync();

            //assert
            result.IsSucess.ShouldBe(false);
            result.Rooms.ShouldBeEmpty();
            result.Message.ShouldBe("Not found");
        }

        [Fact]
        public async Task MustReturnOneRoomById_GetByIdAsync()
        {
            //arrange
            var id = Guid.Parse("0b5786eb-cb60-4e89-bb4a-212d58d5efcd");

            //act
            var result = await service.GetByIdAsync(id);

            //assert
            result.IsSucess.ShouldBe(true);
            result.Room.ShouldNotBeNull();
            result.Message.ShouldBeEmpty();
        }

        private (bool IsSucess, List<RoomResponse> Rooms, string Message) GetAllRoomsActivesMock() =>
             (false, new List<RoomResponse>(), "Not found");

    }
}