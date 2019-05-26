using Moron.Server.Games.WhatIf.Games;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.Players;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Answers
{
    public class AnswerService : IAnswerService
    {
        private static readonly ConcurrentDictionary<Guid, BlockingCollection<Answer>> _answers = new ConcurrentDictionary<Guid, BlockingCollection<Answer>>();
        private readonly IPlayerService _playerService;

        public AnswerService(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public Task<bool> AllAnswersSubmitted(Guid sessionId)
        {
            var result = _answers[sessionId].All(x => x.Submitted);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Answer>> GenerateSessionAnswers(Guid sessionId, IEnumerable<Question> questions)
        {
            var answers = new BlockingCollection<Answer>();
            foreach (var question in questions)
            {
                var answer = new Answer
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    QuestionId = question.Id,
                    CreatedByForeignKey = question.AssignedToPlayerForeignKey
                };
                answers.Add(answer);
            }

            if (_answers.TryGetValue(sessionId, out var answersInSession))
                foreach (var answer in answers)
                {
                    answersInSession.TryAdd(answer);
                }
            else
                _answers.TryAdd(sessionId, answers);
            return Task.FromResult(answers as IEnumerable<Answer>);
        }

        public Task<IEnumerable<Player>> GetPlayersRemainingAnswers(Guid sessionId)
        {
            var answers = _answers[sessionId];
            var unfinishedAnswers = answers.Where(x => !x.Submitted);
            var playerIds = unfinishedAnswers.Select(x => x.CreatedByForeignKey).Distinct();
            return _playerService.Get(playerIds);
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

        public Task<IEnumerable<Answer>> GetPlayerAnswers(Guid sessionId, Guid playerId)
        {
            _answers.TryGetValue(sessionId, out var answersInSession);
            var answers = answersInSession.Where(x => x.CreatedByForeignKey == playerId);
            return Task.FromResult(answers);
        }

        public Task Submit(Guid sessionId, IEnumerable<Answer> answers)
        {
            _answers.TryGetValue(sessionId, out var sessionAnswers);
            foreach (var sessionAnswer in sessionAnswers)
            {
                var updatedAnswer = answers.FirstOrDefault(x => x.Id == sessionAnswer.Id);
                if (updatedAnswer is null)
                    continue;

                sessionAnswer.Text = updatedAnswer.Text;
                sessionAnswer.Submitted = true;
            }

            return Task.CompletedTask;
        }
    }
}
