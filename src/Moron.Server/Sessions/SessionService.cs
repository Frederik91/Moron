using Microsoft.EntityFrameworkCore;
using Moron.Server.Contexts;
using Moron.Server.Helpers;
using Moron.Server.Hubs;
using Moron.Server.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moron.Server.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly IJoinIdGenerator _joinIdGenerator;
        private readonly CommonContext _commonContext;

        public SessionService(IJoinIdGenerator joinIdGenerator, CommonContext commonContext)
        {
            _joinIdGenerator = joinIdGenerator;
            _commonContext = commonContext;
        }

        public async Task<Session> GetAsync(Guid sessionId)
        {
            return await _commonContext.FindAsync<Session>(sessionId);
        }

        public async Task<Session> GetSessionAsync(int joinId)
        {
            var session = await _commonContext.Sessions.AsQueryable().SingleOrDefaultAsync(x => x.JoinId == joinId && !x.Started);
            return session;
        }

        public async Task<Session> CreateAsync(string name, Guid ownerId)
        {
            var owner = _commonContext.Find<Player>(ownerId);
            var session = new Session
            {
                Id = Guid.NewGuid(),
                JoinId = _joinIdGenerator.Generate(),
                Name = name,
                OwnerId = owner.Id,
                PlayersLink = new List<PlayerSession>()
            };
            _commonContext.Sessions.Attach(session);
            var playerSession = new PlayerSession
            {
                PlayerId = owner.Id,
                Player = owner,
                Session = session,
                SessionId = session.Id
            };
            //_commonContext.Attach(playerSession);
            session.PlayersLink.Add(playerSession);
            await _commonContext.SaveChangesAsync();
            return await GetAsync(session.Id);
        }

        public async Task Start(Guid sessionId)
        {
            var session = await GetAsync(sessionId);
            session.Started = true;
            _commonContext.Update(session);
            await _commonContext.SaveChangesAsync();
        }

        public async Task AddPlayerToSession(Guid sessionId, Guid playerId)
        {
            var session = _commonContext.Sessions
            .Include(p => p.PlayersLink)
            .Single(p => p.Id == sessionId);
            var player = _commonContext.Find<Player>(playerId);

            session.PlayersLink.Add(new PlayerSession
            {
                Session = session,
                Player = player,
            });
            //var session = await _commonContext.FindAsync<Session>(sessionId);
            //var player = await _commonContext.FindAsync<Player>(playerId);
            //if (session.PlayersLink is null)
            //    session.PlayersLink = new List<PlayerSession>();
            //session.PlayersLink.Add(new PlayerSession { Player = player, Session = session });
            //if (!_commonContext.ChangeTracker.HasChanges())
            //    _commonContext.Update(session);


            await _commonContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Player>> GetPlayersInSession(Guid sessionId)
        {
            var session = await GetAsync(sessionId);
            return session.PlayersLink.Select(x => x.Player);
        }

        public async Task RemovePlayerFromSession(Guid sessionId, Guid playerId)
        {
            var session = await _commonContext.Sessions.AsQueryable().FirstAsync(x => x.Id == sessionId);
            session.PlayersLink.Remove(session.PlayersLink.First(x => x.PlayerId == playerId));
            await _commonContext.SaveChangesAsync();
        }
    }
}
