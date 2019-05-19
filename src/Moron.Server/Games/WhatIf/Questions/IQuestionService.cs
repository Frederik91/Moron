﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Questions
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> CreateNQuestionsAsync(Guid sessionId, int amount);
        Task Update(IEnumerable<Question> questions);
        Task<bool> AllQuestionsCreated(Guid sessionId);
    }
}
