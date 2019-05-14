using System;

namespace Moron.Server.Sessions
{
    public class GameSession
    {
        public Guid Id { get; set; }
        public int JoinId { get; set; }
        public string Name { get; set; }
    }
}