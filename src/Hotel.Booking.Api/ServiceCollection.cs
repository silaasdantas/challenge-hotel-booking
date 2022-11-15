using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Services;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Hotel.Booking.Api
{
    public static class ServiceCollection
    {
        public static void AddConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();
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
            builder.Services.AddScoped<IRoomRespository, RoomRespository>();
            builder.Services.AddScoped<IBookingRespository, BookingRespository>();
            builder.Services.AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
        }
    }
}
