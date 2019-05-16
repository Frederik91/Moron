using Moron.Server.Helpers;
using Moron.Server.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moron.Server.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly IJoinIdGenerator _joinIdGenerator;
        private static readonly List<ISession> Sessions = new List<ISession>();
        private readonly SessionHub _session;

        public SessionService(IJoinIdGenerator joinIdGenerator, SessionHub session)
        {
            _joinIdGenerator = joinIdGenerator;
            _session = session;
        }

        public Task<ISession> GetAsync(Guid sessionId)
        {
            var session = Sessions.FirstOrDefault(x => x.Id == sessionId);
            return Task.FromResult(session);
        }

        public Task<ISession> GetSessionAsync(int joinId)
        {
            var session = Sessions.FirstOrDefault(x => x.JoinId == joinId);
            return Task.FromResult(session);
        }

        public Task<ISession> CreateAsync(string name)
        {
            ISession session = new GameSession
            {
                Id = Guid.NewGuid(),
                JoinId = _joinIdGenerator.Generate(),
                Name = name
            };
            Sessions.Add(session);
            return Task.FromResult(session);
        }
    }
}
