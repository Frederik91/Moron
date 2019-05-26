using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Games.WhatIf.Questions
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool Submitted { get; set; }
        public Guid SessionId { get; set; }
        public Guid CreatedByForeignKey { get; set; }
        public Guid AssignedToPlayerForeignKey { get; set; }
    }
}
