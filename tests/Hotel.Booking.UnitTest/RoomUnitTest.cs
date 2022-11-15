
using AutoFixture;
using AutoMapper;
using Castle.Core.Logging;
using Hotel.Booking.Core.DTOs;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Shouldly;

namespace Hotel.Booking.UnitTest
{

    public class RoomUnitTest
    {
        


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