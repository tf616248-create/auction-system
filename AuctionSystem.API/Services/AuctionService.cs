using AuctionSystem.API.Data;
using AuctionSystem.API.DTOs;
using AuctionSystem.API.Interfaces;
using AuctionSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionSystem.API.Services;

public class AuctionService : IAuctionService
{
	private readonly AppDbContext _context;

	public AuctionService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<List<AuctionDto>> GetAllAsync()
	{
		return await _context.Auctions
			.Select(a => MapToDto(a))
			.ToListAsync();
	}

	public async Task<AuctionDto?> GetByIdAsync(int id)
	{
		var auction = await _context.Auctions.FindAsync(id);
		return auction is null ? null : MapToDto(auction);
	}

	private static AuctionDto MapToDto(Auction a) => new()
	{
		Id = a.Id,
		Title = a.Title,
		Description = a.Description,
		StartingPrice = a.StartingPrice,
		CurrentPrice = a.CurrentPrice,
		EndTime = a.EndTime,
		Status = a.Status.ToString(),
		TimeRemainingSeconds = (int)Math.Max(0, (a.EndTime - DateTime.UtcNow).TotalSeconds)
	};
}