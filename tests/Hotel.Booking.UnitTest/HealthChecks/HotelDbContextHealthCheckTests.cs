using Hotel.Booking.Api.HealthChecks;
using Hotel.Booking.Core.Entities;
using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shouldly;

namespace Hotel.Booking.Api.HealthChecks.Tests
{
    public class HotelDbContextHealthCheckTests
    {
        [Fact]
        public async Task CheckHealthAsync_ShouldReturnHealthy_WhenDbContextResponds()
        {
            await using var dbContext = CreateDbContext();
            dbContext.Rooms.Add(new RoomEntity(Guid.NewGuid(), "MIO Cancun Hotel Boutique, Queen Suite"));
            await dbContext.SaveChangesAsync();
            var healthCheck = new HotelDbContextHealthCheck(dbContext);

            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            result.Status.ShouldBe(HealthStatus.Healthy);
        }

        private static HotelDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new HotelDbContext(options);
        }
    }
}
