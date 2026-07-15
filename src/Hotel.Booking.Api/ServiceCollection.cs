using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Services;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Hotel.Booking.Api.HealthChecks;
using Hotel.Booking.Api.Shared;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace Hotel.Booking.Api
{
    public static class ServiceCollection
    {
        public const string SensitiveEndpointRateLimitPolicy = "sensitive-endpoint";

        public static void AddConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddRouting(opt => opt.LowercaseUrls = true);
            builder.Services.AddHealthChecks()
                .AddCheck<HotelDbContextHealthCheck>("hotel-db-context");
            builder.Services.AddRateLimiter(opt =>
            {
                opt.AddPolicy(SensitiveEndpointRateLimitPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                opt.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    var result = new CustomResult(
                        HttpStatusCode.TooManyRequests,
                        false,
                        new[] { "Too many requests. Please try again later." });

                    await context.HttpContext.Response.WriteAsJsonAsync(result, cancellationToken);
                };
            });
            builder.Services.AddDbContext<HotelDbContext>(opt => opt.UseInMemoryDatabase("BookingDB"));
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(opt => opt.SuppressMapClientErrors = true)
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    opt.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
            });

            builder.AddServices();
            builder.AddRepositories();
        }

        private static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
        }
        private static void AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
        }
    }
}
