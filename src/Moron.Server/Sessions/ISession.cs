using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Sessions
{
    public interface ISession
    {
        Guid Id { get; }
        int JoinId { get; }
        string Name { get; }
        Guid OwnerId { get; }
        bool Started { get; set; }
    }
}
