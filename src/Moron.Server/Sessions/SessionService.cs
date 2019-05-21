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
        private static readonly List<ISession> _sessions = new List<ISession>();

        public SessionService(IJoinIdGenerator joinIdGenerator)
        {
            _joinIdGenerator = joinIdGenerator;
        }

        public Task<ISession> GetAsync(Guid sessionId)
        {
            var session = _sessions.FirstOrDefault(x => x.Id == sessionId);
            return Task.FromResult(session);
        }

        public Task<ISession> GetSessionAsync(int joinId)
        {
            var session = _sessions.FirstOrDefault(x => x.JoinId == joinId);
            return Task.FromResult(session);
        }

        public Task<ISession> CreateAsync(string name, Guid ownerId)
        {
            ISession session = new GameSession
            {
                Id = Guid.NewGuid(),
                JoinId = _joinIdGenerator.Generate(),
                Name = name,
                OwnerId = ownerId
            };
            _sessions.Add(session);
            return Task.FromResult(session);
        }

        public Task Start(Guid sessionId)
        {
            var session = _sessions.First(x => x.Id == sessionId);
            session.Started = true;
            return Task.CompletedTask;
        }
    }
}
