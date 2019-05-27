using Microsoft.EntityFrameworkCore;
using Moron.Server.Contexts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Players
{
    public class PlayerService : IPlayerService
    {
        //private static readonly ConcurrentDictionary<Guid, Player> _players = new ConcurrentDictionary<Guid, Player>();
        private readonly CommonContext _commonContext;

        public PlayerService(CommonContext commonContext)
        {
            _commonContext = commonContext;
        }

        public async Task<Player> Create(string name)
        {
            var player = new Player { Id = Guid.NewGuid(), Name = name, SessionsLink = new List<Sessions.PlayerSession>() };
            _commonContext.Add(player);
            await _commonContext.SaveChangesAsync();
            return await Get(player.Id);
        }

        public async Task<Player> Get(Guid id)
        {
            var res = await _commonContext.FindAsync<Player>(id);
            return res;
        }

        public async Task<IEnumerable<Player>> Get(IEnumerable<Guid> ids)
        {
            var players = await _commonContext.Players.Where(x => ids.Contains(x.Id)).ToListAsync();
            return players;
        }
    }
}
