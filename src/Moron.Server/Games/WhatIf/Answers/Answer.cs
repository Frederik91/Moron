using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Games.WhatIf.Answers
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public Guid QuestionId { get; set; }
        public bool Submitted { get; set; }
        public string Text { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
