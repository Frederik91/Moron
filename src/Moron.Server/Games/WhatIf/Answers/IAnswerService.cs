using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.Players;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Answers
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> GenerateSessionAnswers(Guid sessionId, IEnumerable<Question> questions);
        Task Submit(Guid sessionId, IEnumerable<Answer> answers);
        Task<IEnumerable<Answer>> GetPlayerAnswers(Guid sessionId, Guid playerId);
        Task<bool> AllAnswersSubmitted(Guid sessionId);
        Task<IEnumerable<Answer>> GetAnswersInSession(Guid sessionId);
        Task<Answer> Get(Guid sessionId, Guid answerId);
        Task<IEnumerable<Player>> GetPlayersRemainingAnswers(Guid sessionId);
    }
}
