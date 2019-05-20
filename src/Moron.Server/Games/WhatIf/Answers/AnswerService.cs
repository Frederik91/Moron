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

        public Task<Answer> GetRandomAnswer(Guid sessionId, Guid questionId)
        {
            var sessionAnswers = _answers[sessionId];
            var answer = sessionAnswers.FirstOrDefault(x => x.QuestionId != questionId && !x.Used);
            answer.Used = true;
            return Task.FromResult(answer);
        }

        public Task Submit(IEnumerable<Answer> answers)
        {
            var allAnswers = _answers.SelectMany(x => x.Value).ToDictionary(x => x.Id);
            foreach (var answer in answers)
            {
                allAnswers[answer.Id].Text = answer.Text;
                allAnswers[answer.Id].Submitted = true;
            }
            return Task.CompletedTask;
        }
    }
}
