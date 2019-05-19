using Moron.Server.Sessions;
using System;

namespace Moron.Server.Sessions
{
    public class GameSession : ISession
    {
        public Guid Id { get; set; }
        public int JoinId { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public bool Started { get; set; }
    }
}