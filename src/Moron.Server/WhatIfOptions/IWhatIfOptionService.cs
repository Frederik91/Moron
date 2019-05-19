using System;
using System.Threading.Tasks;

namespace Moron.Server.WhatIfOptions
{
    public interface IWhatIfOptionService
    {
        Task<WhatIfOption> Create(Guid sessionId);
        Task<WhatIfOption> Get(Guid sessionId);
        Task Update(Guid sessionId, WhatIfOption option);
    }
}