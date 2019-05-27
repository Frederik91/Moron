using Moron.Server.Players;
using Moron.Server.Sessions;
using System;
using System.Collections.Generic;

namespace Moron.Server.Sessions
{
    public class Session
    {
        public Guid Id { get; set; }
        public int JoinId { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public virtual List<PlayerSession> PlayersLink { get; set; }
        public bool Started { get; set; }
    }
}