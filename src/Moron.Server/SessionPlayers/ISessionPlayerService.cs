using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.SessionPlayers
{
    public interface ISessionPlayerService
    {
        Task AddPlayerToSession(Guid sessionId, Guid playerId);
        Task<IEnumerable<Guid>> GetPlayersInSession(Guid sessionId);
        Task RemovePlayerFromSession(Guid sessionId, Guid playerId);
    }
}
