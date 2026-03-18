using AuctionSystem.API.Interfaces;

namespace AuctionSystem.API.Routes;

public static class AuctionRoutes
{
    public static void MapAuctionRoutes(this WebApplication app)
    {
        app.MapGet("/api/auctions", async (IAuctionService service) =>
        {
            var auctions = await service.GetAllAsync();
            return Results.Ok(auctions);
        });

        app.MapGet("/api/auctions/{id}", async (int id, IAuctionService service) =>
        {
            var auction = await service.GetByIdAsync(id);
            return auction is null ? Results.NotFound() : Results.Ok(auction);
        });
    }
}