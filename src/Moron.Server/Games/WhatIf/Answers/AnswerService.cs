using Moron.Server.Games.WhatIf.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Answers
{
    public class AnswerService : IAnswerService
    {
        private static readonly Dictionary<Guid, List<Answer>> _answers = new Dictionary<Guid, List<Answer>>();

        public Task<bool> AllAnswersSubmitted(Guid sessionId)
        {
            var result = _answers[sessionId].All(x => x.Submitted);
            return Task.FromResult(result);
        }

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
                answers.Add(answer);
            }
            if (_answers.ContainsKey(sessionId))
                _answers[sessionId].AddRange(answers);
            else
                _answers.Add(sessionId, answers);
            return Task.FromResult(answers as IEnumerable<Answer>);
        }

        public Task<Answer> Get(Guid sessionId, Guid answerId)
        {
            var answer = _answers[sessionId].FirstOrDefault(x => x.Id == answerId);
            return Task.FromResult(answer);
        }

        public Task<IEnumerable<Answer>> GetAnswersInSession(Guid sessionId)
        {
            return Task.FromResult(_answers[sessionId] as IEnumerable<Answer>);
        }

        public Task Submit(IEnumerable<Answer> answers, Guid playerId)
        {
            var allAnswers = _answers.SelectMany(x => x.Value).ToDictionary(x => x.Id);
            foreach (var answer in answers)
            {
                allAnswers[answer.Id].Text = answer.Text;
                allAnswers[answer.Id].Submitted = true;
                allAnswers[answer.Id].CreatedBy = playerId;
            }

            return Task.CompletedTask;
        }
    }
}
