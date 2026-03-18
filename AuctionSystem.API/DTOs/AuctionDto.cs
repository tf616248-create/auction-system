namespace AuctionSystem.API.DTOs;

public class AuctionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TimeRemainingSeconds { get; set; }
}

public class PlaceBidDto
{
    public string BidderName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class BidDto
{
    public int Id { get; set; }
    public string BidderName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
}