using AutoMapper;
using Hotel.Booking.Core.Profiles;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Hotel.Booking.Core.Services.Tests
{
    public class RoomServiceTests
    {
        [Fact]
        public async Task MustReturnAListRooms_GetAllRoomsActives()
        {
            //arrange
            var options = new DbContextOptionsBuilder<HotelDbContext>()
               .UseInMemoryDatabase(nameof(MustReturnAListRooms_GetAllRoomsActives))
               .Options;
            var dbContext = new HotelDbContext(options);
            var respository = new RoomRespository(dbContext);
            var roomProfile = new RoomProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(roomProfile));
            var mapper = new Mapper(configuration);
            var service = new RoomService(respository, mapper);

            //act
            var expected = await service.GetAllRoomsActivesAsync();

            //assert
            expected.IsSucess.ShouldBe(true);
            expected.Rooms.ShouldNotBeEmpty();
            expected.Message.ShouldBeEmpty();
        }

        [Fact]
        public async Task MustReturnAListRoomsEmpty_GetAllRoomsActives()
        {
            //arrange
            var options = new DbContextOptionsBuilder<HotelDbContext>()
              .UseInMemoryDatabase(nameof(MustReturnAListRoomsEmpty_GetAllRoomsActives))
              .Options;
            var dbContext = new HotelDbContext(options);
            var respository = new RoomRespository(dbContext);
            var roomProfile = new RoomProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(roomProfile));
            var mapper = new Mapper(configuration);
            var service = new RoomService(respository, mapper);

            //act
            var expected = await service.GetAllRoomsActivesAsync();

            //assert
            expected.IsSucess.ShouldBe(false);
            expected.Rooms.ShouldBeEmpty();
            expected.Message.ShouldNotBeEmpty();
        }
   
    }
}