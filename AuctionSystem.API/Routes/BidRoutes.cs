using AuctionSystem.API.DTOs;
using AuctionSystem.API.Interfaces;

namespace AuctionSystem.API.Routes;

public static class BidRoutes
{
    public static void MapBidRoutes(this WebApplication app)
    {
        app.MapPost("/api/auctions/{id}/bids", async (int id, PlaceBidDto dto, IBidService service) =>
        {
            try
            {
                var bid = await service.PlaceBidAsync(id, dto);
                return Results.Ok(bid);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }
}