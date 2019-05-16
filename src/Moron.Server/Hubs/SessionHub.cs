using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Hubs
{
    public class SessionHub : Hub
    {
        public async Task AddToSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            await Clients.Group(sessionId).SendAsync("Send", $"{Context.ConnectionId} has joined the group {sessionId}.");
        }

        public async Task RemoveFromSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);

            await Clients.Group(sessionId).SendAsync("Send", $"{Context.ConnectionId} has left the group {sessionId}.");
        }
    }
}
