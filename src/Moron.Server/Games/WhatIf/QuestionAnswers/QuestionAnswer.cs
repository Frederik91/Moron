using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Games.WhatIf.QuestionAnswers
{
    public class QuestionAnswer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
    }
}
