using Hotel.Booking.Api.Controllers;
using Hotel.Booking.Api.Shared;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Hotel.Booking.Api.Controllers.Tests
{
    public class BookingControllerTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnGenericMessage_WhenUnexpectedErrorOccurs()
        {
            var serviceMock = new Mock<IBookingService>();
            serviceMock.Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("database internal failure"));
            var loggerMock = new Mock<ILogger<BookingController>>();
            var controller = new BookingController(serviceMock.Object, loggerMock.Object);

            var actionResult = await controller.GetAllAsync(CancellationToken.None);

            var jsonResult = actionResult.ShouldBeOfType<JsonResult>();
            jsonResult.StatusCode.ShouldBe(400);
            var customResult = jsonResult.Value.ShouldBeOfType<CustomResult>();
            customResult.Success.ShouldBeFalse();
            customResult.Errors.ShouldContain("An unexpected error occurred.");
            customResult.Errors.ShouldNotContain("database internal failure");
        }

        [Fact]
        public async Task GetAllAsync_ShouldPreserveExpectedServiceFailure()
        {
            var serviceMock = new Mock<IBookingService>();
            serviceMock.Setup(_ => _.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, ServiceResultStatus.NotFound, new List<Core.DTOs.BookingResponse>(), "Not found"));
            var loggerMock = new Mock<ILogger<BookingController>>();
            var controller = new BookingController(serviceMock.Object, loggerMock.Object);

            var actionResult = await controller.GetAllAsync(CancellationToken.None);

            var jsonResult = actionResult.ShouldBeOfType<JsonResult>();
            jsonResult.StatusCode.ShouldBe(404);
            var customResult = jsonResult.Value.ShouldBeOfType<CustomResult>();
            customResult.Errors.ShouldContain("Not found");
        }
    }
}
