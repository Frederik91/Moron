using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.WhatIfOptions
{
    public class WhatIfOptionService : IWhatIfOptionService
    {
        private static readonly Dictionary<Guid, WhatIfOption> _options = new Dictionary<Guid, WhatIfOption>();

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
            _options.Add(sessionId, option);
            return Task.FromResult(option);
        }

        public Task Update(Guid sessionId, WhatIfOption option)
        {
            _options[sessionId] = option;
            return Task.CompletedTask;
        }
    }
}
