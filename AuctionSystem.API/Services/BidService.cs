using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs;
using AuctionSystem.API.Hubs;
using AuctionSystem.API.Interfaces;
using AuctionSystem.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Services;

public class BidService : IBidService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<AuctionHub> _hubContext;

    public BidService(AppDbContext context, IHubContext<AuctionHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<BidDto> PlaceBidAsync(int auctionId, PlaceBidDto dto)
    {
        var auction = await _context.Auctions.FindAsync(auctionId)
            ?? throw new KeyNotFoundException("מכירה לא נמצאה");

        if (auction.Status == AuctionStatus.Closed)
            throw new InvalidOperationException("המכירה נסגרה");

        if (auction.EndTime <= DateTime.UtcNow)
            throw new InvalidOperationException("זמן המכירה פג");

        if (dto.Amount <= auction.CurrentPrice)
            throw new InvalidOperationException($"ההצעה חייבת להיות גבוהה מ-{auction.CurrentPrice}");

        var bid = new Bid
        {
            AuctionId = auctionId,
            BidderName = dto.BidderName,
            Amount = dto.Amount,
            PlacedAt = DateTime.UtcNow
        };

        auction.CurrentPrice = dto.Amount;
        _context.Bids.Add(bid);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // מישהו אחר הגיש הצעה גבוהה יותר באותו רגע
            await _context.Entry(auction).ReloadAsync();
            throw new InvalidOperationException($"המחיר התעדכן ל-{auction.CurrentPrice}, נסה שוב");
        }

        // עדכון כל המשתמשים המחוברים לאותה מכירה
        await _hubContext.Clients
            .Group($"auction_{auctionId}")
            .SendAsync("BidPlaced", new
            {
                auctionId,
                newPrice = auction.CurrentPrice,
                bidderName = dto.BidderName,
                amount = dto.Amount
            });

        await _hubContext.Clients
            .Group("auction_list")
            .SendAsync("BidPlaced", new { auctionId, newPrice = auction.CurrentPrice, bidderName = dto.BidderName, amount = dto.Amount }); ;

        return new BidDto
        {
            Id = bid.Id,
            BidderName = bid.BidderName,
            Amount = bid.Amount,
            PlacedAt = bid.PlacedAt
        };
    }
}