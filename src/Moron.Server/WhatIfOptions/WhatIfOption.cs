using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.WhatIfOptions
{
    public class WhatIfOption
    {
        public int NumberOfCards { get; set; }
        public Guid Id { get; internal set; }
    }
}
