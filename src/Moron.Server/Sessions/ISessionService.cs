using Moron.Server.Players;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Sessions
{
    public interface ISessionService
    {
        Task<Session> GetAsync(Guid sessionId);
        Task<Session> GetSessionAsync(int sessionId);
        Task<Session> CreateAsync(string name, Guid ownerId);
        Task Start(Guid sessionId);
        Task AddPlayerToSession(Guid sessionId, Guid playerId);
        Task<IEnumerable<Player>> GetPlayersInSession(Guid sessionId);
        Task RemovePlayerFromSession(Guid sessionId, Guid playerId);
    }
}
