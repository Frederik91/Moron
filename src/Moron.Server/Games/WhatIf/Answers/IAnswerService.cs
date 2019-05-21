using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Answers
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> CreateAnswers(Guid sessionId, IEnumerable<Guid> questionIds);
        Task Submit(IEnumerable<Answer> answers, Guid playerId);
        Task<bool> AllAnswersSubmitted(Guid sessionId);
        Task<IEnumerable<Answer>> GetAnswersInSession(Guid sessionId);
        Task<Answer> Get(Guid sessionId, Guid answerId);
    }
}
