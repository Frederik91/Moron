using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Games.WhatIf.Games
{
    public class Game
    {
        public bool IsFinished { get; set; }
        public List<Turn> Turns { get; set; }
    }
}
