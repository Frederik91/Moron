using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Sessions
{
    public interface ISessionService
    {
        Task<ISession> GetAsync(Guid sessionId);
        Task<ISession> GetSessionAsync(int sessionId);
        Task<ISession> CreateAsync(string name, Guid ownerId);
        Task Start(Guid sessionId);
    }
}
