using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.QuestionAnswers
{
    public interface IQuestionAnswerService
    {
        Task AddQuestionsToSession(Guid sessionId, IEnumerable<Guid> questionIds);
        Task<IEnumerable<QuestionAnswer>> GetQuestionsInSession(Guid sessionId);
    }
}