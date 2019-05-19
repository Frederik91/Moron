using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Answers
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> CreateAnswers(Guid sessionId, IEnumerable<Guid> questionIds);
        Task Update(IEnumerable<Answer> answers);
        Task<Answer> GetRandomAnswer(Guid sessionId, Guid questionId);
    }
}
