using Microsoft.AspNetCore.SignalR;

namespace AuctionSystem.API.Hubs;

public class AuctionHub : Hub
{
    public async Task JoinAuction(string auctionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"auction_{auctionId}");
    }

    public async Task LeaveAuction(string auctionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"auction_{auctionId}");
    }

  
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "auction_list");
        await base.OnConnectedAsync();
    }
}