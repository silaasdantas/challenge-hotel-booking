using Hotel.Booking.Api;
using Hotel.Booking.Core.Interfaces;
using Hotel.Booking.Core.Services;
using Hotel.Booking.Infra.Data.Db;
using Hotel.Booking.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddConfigureServices();

const string frontendCorsPolicy = "FrontendDevelopment";

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors(frontendCorsPolicy);
}

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/self");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    DbSeeds.SeedData(dbContext);
}

app.Run();

