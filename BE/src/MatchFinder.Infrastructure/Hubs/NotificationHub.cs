using MatchFinder.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MatchFinder.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub : Hub<INotificationHub>
    {
        public override async Task OnConnectedAsync()
        {
            var claimsIdentity = Context?.User?.Identity as ClaimsIdentity;
            var idClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"UserID_{idClaim.Value}");
            await base.OnConnectedAsync();
        }
    }
}