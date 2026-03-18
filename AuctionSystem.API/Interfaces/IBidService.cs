using AuctionSystem.API.DTOs;

namespace AuctionSystem.API.Interfaces;

public interface IBidService
{
    Task<BidDto> PlaceBidAsync(int auctionId, PlaceBidDto dto);
}