
using AutoFixture;
using AutoMapper;
using Castle.Core.Logging;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Hotel.Booking.UnitTest
{
    //As pessoas agora est�o livres para viajar para todos os lugares, mas por causa da pandemia, muitos hot�is faliram.
    //Alguns antigos lugares de viagens famosos ficam com apenas um hotel.Voc� recebeu a responsabilidade de desenvolver
    //uma API de reservas para o �ltimo hotel em Cancun.

    //Os requisitos s�o:
    //- API ser� mantida pelo departamento de TI do hotel.
    //- Como � o �ltimo hotel, a qualidade do servi�o deve ser de 99,99 a 100% => sem downtime
    //- Para efeitos do teste, assumimos que o hotel tem apenas um quarto dispon�vel
    //- Para dar oportunidade a todos de reservarem o quarto, a estadia n�o pode ser superior a 3 dias
    // e n�o pode ser reservado com mais de 30 dias de anteced�ncia.
    //- Todas as reservas come�am pelo menos no dia seguinte � reserva,
    //- Para simplificar o caso de uso, um �DIA� no quarto do hotel come�a das 00:00 �s 23:59:59.
    //- Cada usu�rio final pode verificar a disponibilidade do quarto, fazer uma reserva, cancel�-la ou modific�-la.
    //- Para simplificar a API � inseguro.

    public class RoomUnitTest
    {

        [Fact]
        public async void MustGetAllRoomsActives()
        {
            //arrange
            var bookingRequest = new Fixture().Create<BookingRequest>();
            bookingRequest.CheckIn = DateTime.Now.AddDays(1);
            bookingRequest.CheckOut = DateTime.Now.AddDays(4);

            var repositoryMock = new Mock<IRoomRespository>();
            var loggerMock = new Mock<ILogger<RoomService>>();
            var mapperMock = new Mock<IMapper>();
            var service = new RoomService(repositoryMock.Object, loggerMock.Object, mapperMock.Object);

            //act
            var result = await service.GetAllRoomsActivesAsync();

            //assert
            result.IsSucess.ShouldBe(true);
            result.Rooms.ShouldNotBeNull();
            result.Message.ShouldNotBeNull();
        }


        //[Fact]
        //public void MustValidateABooking()
        //{
        //    var checkInDate = DateTime.Now.AddDays(1);
        //    var checkOutDate = DateTime.Now.AddDays(4);

        //    var result = _room.IsValidBooking(checkInDate, checkOutDate);

        //    Assert.True(result);
        //}

        //[Fact]
        //public void MustValidateABookThatDoesnotStartTheNextDay()
        //{
        //    var checkInDate = DateTime.Now;
        //    var checkOutDate = DateTime.Now.AddDays(3);

        //    var result = _room.IsValidBooking(checkInDate, checkOutDate);

        //    Assert.False(result);
        //}

        //[Fact]
        //public void MustValidateRoomsBookingOver30DaysInAdvance()
        //{
        //    var checkInDate = DateTime.Now.AddDays(31);
        //    var checkOutDate = DateTime.Now.AddDays(32);

        //    var result = _room.IsValidBooking(checkInDate, checkOutDate);

        //    Assert.False(result);
        //}

        //[Fact]
        //public void MustValidateACheckoutDateMinorThanCheckIn()
        //{
        //    var checkInDate = DateTime.Now.AddDays(3);
        //    var checkOutDate = DateTime.Now;

        //    var result = _room.IsValidBooking(checkInDate, checkOutDate);

        //    Assert.False(result);
        //}

        //[Fact]
        //public void MustValidateRoomsWithBookingAboveStayLimit()
        //{
        //    var checkInDate = DateTime.Now.AddDays(1);
        //    var checkOutDate = DateTime.Now.AddDays(5);

        //    var result = _room.IsValidBooking(checkInDate, checkOutDate);

        //    Assert.False(result);
        //}


        //[Fact]
        //public void MustValidateRoomsWithoutBooking()
        //{
        //    var checkIn = DateTime.Now;
        //    var checkOut = DateTime.Now.AddDays(3);

        //    var expectedValidateBooking = _room.IsValidBooking(checkIn, checkOut);
        //    //var expectedHasBooking = _room.Bookings.Any(_ => _.CheckIn.Date <= checkOut.Date && _.CheckOut <= checkIn);

        //    //Assert.False(expectedHasBooking);
        //    //Assert.False(expectedHasBooking);
        //}

        //[Fact]
        //public void MustReturnRoomsWithBookingForTheNextDay()
        //{
        //    var checkIn = DateTime.Now.AddDays(1);
        //    var checkOut = DateTime.Now.AddDays(3);

        //    //var hasBooking = _room.Bookings.Any(_ => _.CheckIn.Date <= checkOut.Date && _.CheckOut <= checkIn);

        //    //Assert.False(hasBooking);
        //}

        //[Fact]
        //public void CancelBooking()
        //{

        //}

        //[Fact]
        //public void UpdateBooking()
        //{

        //}
    }

}