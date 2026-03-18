using AuctionSystem.API.DTOs;

namespace AuctionSystem.API.Interfaces;

public interface IAuctionService
{
    Task<List<AuctionDto>> GetAllAsync();
    Task<AuctionDto?> GetByIdAsync(int id);
}