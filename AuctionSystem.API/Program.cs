using AuctionSystem.API.Data;
using AuctionSystem.API.Hubs;
using AuctionSystem.API.Interfaces;
using AuctionSystem.API.Routes;
using AuctionSystem.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddHostedService<AuctionExpiryService>(); // ← כאן


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

app.UseCors("AllowAngular");

app.MapHub<AuctionHub>("/hubs/auction");
app.MapAuctionRoutes();
app.MapBidRoutes();

app.Run();