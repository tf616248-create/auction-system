namespace AuctionSystem.API.Models;

public class Bid
{
    public int Id { get; set; }
    public int AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;
    public string BidderName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
}