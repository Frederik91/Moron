using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Players
{
    public interface IPlayerService
    {
        Task<Player> Get(Guid id);
        Task<Player> Create(string nickname);
        Task<IEnumerable<Player>> Get(IEnumerable<Guid> ids);
    }
}
