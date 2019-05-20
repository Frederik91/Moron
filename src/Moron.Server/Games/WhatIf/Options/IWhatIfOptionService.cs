using System;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Options
{
    public interface IWhatIfOptionService
    {
        Task<WhatIfOption> Create(Guid sessionId);
        Task<WhatIfOption> Get(Guid sessionId);
        Task Update(Guid sessionId, WhatIfOption option);
    }
}