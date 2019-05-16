using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Players
{
    public class PlayerService : IPlayerService
    {
        private static readonly Dictionary<Guid, Player> _players = new Dictionary<Guid, Player>();

        public Task<Player> Create(string name)
        {
            var player = new Player { Id = Guid.NewGuid(), Name = name };
            _players.Add(player.Id, player);
            return Task.FromResult(player);
        }

        public Task<Player> Get(Guid id)
        {
            _players.TryGetValue(id, out var player);
            return Task.FromResult(player);
        }

        public Task<IEnumerable<Player>> Get(IEnumerable<Guid> ids)
        {
            var players = ids.Where(x => _players.ContainsKey(x)).Select(x => _players[x]);
            return Task.FromResult(players);
        }
    }
}
