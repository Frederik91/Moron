using Moron.Server.Sessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Moron.Server.Players
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual List<PlayerSession> SessionsLink { get; set; }
    }
}
