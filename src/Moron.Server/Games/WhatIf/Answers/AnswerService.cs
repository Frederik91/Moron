using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Answers
{
    public class AnswerService : IAnswerService
    {
        private static readonly Dictionary<Guid, Answer> _answers = new Dictionary<Guid, Answer>();

        public Task<IEnumerable<Answer>> CreateAnswers(Guid sessionId, IEnumerable<Guid> questionIds)
        {
            var answers = new List<Answer>();
            foreach (var questionId in questionIds)
            {
                var answer = new Answer
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    QuestionId = questionId
                };
                _answers.Add(answer.Id, answer);
                answers.Add(answer);
            }
            return Task.FromResult(answers as IEnumerable<Answer>);
        }

        public Task<Answer> GetRandomAnswer(Guid sessionId, Guid questionId)
        {
            var sessionAnswers = _answers.Values.Where(x => x.SessionId == sessionId);
            var availableAnswers = sessionAnswers.Where(x => !x.Used);
            return Task.FromResult(availableAnswers.First(x => x.QuestionId != questionId));
        }

        public Task Update(IEnumerable<Answer> answers)
        {
            foreach (var answer in answers)
            {
                _answers[answer.Id] = answer;
            }
            return Task.CompletedTask;
        }
    }
}
