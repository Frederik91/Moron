using Microsoft.AspNetCore.SignalR;
using Moron.Server.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.SessionPlayers
{
    public class SessionPlayerService : ISessionPlayerService
    {
        private static readonly ConcurrentDictionary<Guid, List<Guid>> _sessionUsers = new ConcurrentDictionary<Guid, List<Guid>>();
        private readonly IHubContext<SessionHub> _sessionHub;

        public SessionPlayerService(IHubContext<SessionHub> sessionHub)
        {
            _sessionHub = sessionHub;
        }

        public Task AddPlayerToSession(Guid sessionId, Guid playerId)
        {
            if (_sessionUsers.TryGetValue(sessionId, out var playerIds))
                playerIds.Add(playerId);
            else
                _sessionUsers.TryAdd(sessionId, new List<Guid> { playerId });

            return _sessionHub.Groups.AddToGroupAsync(playerId.ToString(), sessionId.ToString());
        }

        public Task<IEnumerable<Guid>> GetPlayersInSession(Guid sessionId)
        {
            _sessionUsers.TryGetValue(sessionId, out var playerIds);
            return Task.FromResult(playerIds ?? new List<Guid>() as IEnumerable<Guid>);
        }

        public Task RemovePlayerFromSession(Guid sessionId, Guid playerId)
        {
            if (_sessionUsers.TryGetValue(sessionId, out var playerIds))
                playerIds.Remove(playerId);

            return _sessionHub.Groups.RemoveFromGroupAsync(sessionId.ToString(), playerId.ToString());
        }

        public void SubscribeToUsersJoiningSession()
        {

        }
    }
}
