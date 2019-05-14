using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Sessions
{
    public interface ISessionService
    {
        Task<GameSession> GetAsync(Guid sessionId);
        Task<GameSession> GetSessionAsync(int sessionId);
        Task<GameSession> CreateAsync(string name);
    }
}
