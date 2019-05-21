using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Games.WhatIf.Games
{
    public class Turn
    {
        public Guid Id { get; set; }
        public bool IsFinished { get; set; }
        public Guid QuestionId { get; set; }
        public Guid PlayerQuestionId { get; set; }
        public Guid AnswerId { get; set; }
        public Guid PlayerAnswerId { get; set; }
    }
}
