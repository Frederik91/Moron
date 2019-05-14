using Moron.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly IJoinIdGenerator _joinIdGenerator;
        private static readonly List<GameSession> Sessions = new List<GameSession>();
             
        public SessionService(IJoinIdGenerator joinIdGenerator)
        {
            _joinIdGenerator = joinIdGenerator;
        }

        public Task<GameSession> GetAsync(Guid sessionId)
        {
            var session = Sessions.FirstOrDefault(x => x.Id == sessionId);
            return Task.FromResult(session);
        }

        public Task<GameSession> GetSessionAsync(int joinId)
        {
            var session = Sessions.FirstOrDefault(x => x.JoinId == joinId);
            return Task.FromResult(session);
        }

        public Task<GameSession> CreateAsync(string name)
        {
            var session = new GameSession
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
