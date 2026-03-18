using AuctionSystem.API.Data;
using AuctionSystem.API.Hubs;
using AuctionSystem.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Services;

public class AuctionExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<AuctionHub> _hubContext;

    public AuctionExpiryService(IServiceScopeFactory scopeFactory, IHubContext<AuctionHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckExpiredAuctions();
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task CheckExpiredAuctions()
    {
        // BackgroundService הוא Singleton – צריך Scope ידני לDbContext
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var expiredAuctions = await context.Auctions
            .Where(a => a.Status == AuctionStatus.Active && a.EndTime <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var auction in expiredAuctions)
        {
            auction.Status = AuctionStatus.Closed;

            // עדכון כל המשתמשים המחוברים
            await _hubContext.Clients
                .Group($"auction_{auction.Id}")
                .SendAsync("AuctionClosed", new
                {
                    auctionId = auction.Id,
                    finalPrice = auction.CurrentPrice
                });
        }

        if (expiredAuctions.Any())
            await context.SaveChangesAsync();
    }
}