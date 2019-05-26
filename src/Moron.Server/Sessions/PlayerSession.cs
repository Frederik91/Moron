using Moron.Server.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Sessions
{
    public class PlayerSession
    {
        public Guid SessionId { get; set; }
        public Guid PlayerId { get; set; }

        public virtual Session Session { get; set; }
        public virtual Player Player { get; set; }
    }
}
