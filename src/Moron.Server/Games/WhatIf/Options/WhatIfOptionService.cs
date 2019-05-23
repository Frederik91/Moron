using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Options
{
    public class WhatIfOptionService : IWhatIfOptionService
    {
        private static readonly ConcurrentDictionary<Guid, WhatIfOption> _options = new ConcurrentDictionary<Guid, WhatIfOption>();

        public Task<WhatIfOption> Get(Guid sessionId)
        {
            _options.TryGetValue(sessionId, out var option);
            return Task.FromResult(option);
        }

        public Task<WhatIfOption> Create(Guid sessionId)
        {
            var option = new WhatIfOption()
            {
                Id = Guid.NewGuid(),
                NumberOfCards = 3,
            };
            _options.TryAdd(sessionId, option);
            return Task.FromResult(option);
        }

        public Task Update(Guid sessionId, WhatIfOption option)
        {
            _options[sessionId] = option;
            return Task.CompletedTask;
        }
    }
}
