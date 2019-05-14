using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Helpers
{
    public class JoinIdGenerator : IJoinIdGenerator
    {
        private static readonly Random _rnd = new Random(10);

        public int Generate()
        {
            return _rnd.Next(1000, 9999);
        }
    }
}
