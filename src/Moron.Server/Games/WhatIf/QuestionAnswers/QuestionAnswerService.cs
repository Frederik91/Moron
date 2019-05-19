using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.QuestionAnswers
{
    public class QuestionAnswerService : IQuestionAnswerService
    {
        private static readonly Dictionary<Guid, List<QuestionAnswer>> _questionAnswer = new Dictionary<Guid, List<QuestionAnswer>>();

        public Task AddQuestionsToSession(Guid sessionId, IEnumerable<Guid> questionIds)
        {
            var questionAnswers = questionIds.Select(x => new QuestionAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = x
            });
            if (_questionAnswer.ContainsKey(sessionId))
                _questionAnswer[sessionId].AddRange(questionAnswers);
            else
                _questionAnswer.Add(sessionId, questionAnswers.ToList());

            return Task.CompletedTask;
        }

        public Task<IEnumerable<QuestionAnswer>> GetQuestionsInSession(Guid sessionId)
        {
            return Task.FromResult(_questionAnswer[sessionId] as IEnumerable<QuestionAnswer>);
        }
    }
}
