using Moron.Server.Sessions;
using System;

namespace Moron.Server.Sessions
{
    public class GameSession : ISession
    {
        public Guid Id { get; set; }
        public int JoinId { get; set; }
        public string Name { get; set; }

        public void AddPlayer(Guid userId)
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(Guid userId)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}