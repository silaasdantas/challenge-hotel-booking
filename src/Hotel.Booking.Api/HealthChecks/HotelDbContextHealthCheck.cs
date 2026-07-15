using Hotel.Booking.Infra.Data.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hotel.Booking.Api.HealthChecks
{
    public class HotelDbContextHealthCheck : IHealthCheck
    {
        private readonly HotelDbContext _dbContext;

        public HotelDbContextHealthCheck(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.Rooms.AsNoTracking().AnyAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Hotel database context is unavailable.", ex);
            }
        }
    }
}
