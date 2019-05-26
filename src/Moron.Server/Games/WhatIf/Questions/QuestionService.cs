using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Players;
using Moron.Server.Sessions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Questions
{
    public class QuestionService : IQuestionService
    {
        private static readonly ConcurrentDictionary<Guid, List<Question>> _questions = new ConcurrentDictionary<Guid, List<Question>>();
        private readonly IWhatIfOptionService _whatIfOptionService;
        private readonly IPlayerService _playerService;
        private readonly ISessionService _sessionService;

        public QuestionService(IWhatIfOptionService whatIfOptionService, IPlayerService playerService, ISessionService sessionService)
        {
            _whatIfOptionService = whatIfOptionService;
            _playerService = playerService;
            _sessionService = sessionService;
        }

        public Task<bool> AllQuestionsSubmitted(Guid sessionId)
        {
            var questions = _questions[sessionId];
            var result = questions.All(x => x.Submitted);
            return Task.FromResult(result);
        }

        public async Task<IEnumerable<Question>> GenerateQuestionsForSession(Guid sessionId)
        {
            var options = await _whatIfOptionService.Get(sessionId);
            var players = (await _sessionService.GetPlayersInSession(sessionId)).ToList();
            var questions = new List<Question>();
            for (int i = 1; i <= options.NumberOfCards; i++)
            {
                var questionsThisRound = new List<Question>();
                foreach (var player in players)
                {
                    var playersAssignedQuestions = questions.GroupBy(x => x.AssignedToPlayerForeignKey);
                    var playersAssignedQuestionThisRound = questionsThisRound.Select(x => x.AssignedToPlayerForeignKey);
                    var validPlayersToAssignTo = players.Where(x => x.PlayerId != player.PlayerId && playersAssignedQuestionThisRound?.Contains(x.PlayerId) != true);

                    var assignToPlayerId = validPlayersToAssignTo.FirstOrDefault()?.PlayerId;
                    if (assignToPlayerId == Guid.Empty)
                    {
                        if (players.Count == 2)
                            assignToPlayerId = players.FirstOrDefault(x => x.PlayerId != player.PlayerId)?.PlayerId;
                        else
                        {
                            var questionToSwap = questionsThisRound.FirstOrDefault();
                            assignToPlayerId = questionToSwap.AssignedToPlayerForeignKey;
                            questionToSwap.AssignedToPlayerForeignKey = player.PlayerId;
                        }
                    }

                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        CreatedByForeignKey = player.PlayerId,
                        SessionId = sessionId,
                        AssignedToPlayerForeignKey = assignToPlayerId.GetValueOrDefault()
                    };
                    questions.Add(question);
                    questionsThisRound.Add(question);
                }
            }
            _questions.TryAdd(sessionId, questions);
            return questions;
        }

        public Task<Question> Get(Guid sessionId, Guid questionId)
        {
            var question = _questions[sessionId].FirstOrDefault(x => x.Id == questionId);
            return Task.FromResult(question);
        }

        public Task<IEnumerable<Player>> GetPlayersRemainingQuestions(Guid sessionId)
        {
            var questions = _questions[sessionId];
            var unfinishedQuestions = questions.Where(x => !x.Submitted);
            var playerIds = unfinishedQuestions.Select(x => x.CreatedByForeignKey).Distinct();
            return _playerService.Get(playerIds);
        }

        public Task<IEnumerable<Question>> GetQuestionsAssignedToPlayer(Guid sessionId, Guid playerId)
        {
            var questionsInSession = _questions[sessionId];
            var questions = questionsInSession.Where(x => x.AssignedToPlayerForeignKey == playerId);
            return Task.FromResult(questions);
        }

        public Task<IEnumerable<Question>> GetQuestionsCreatedByPlayer(Guid sessionId, Guid playerId)
        {
            var questionsInSession = _questions[sessionId];
            var questions = questionsInSession.Where(x => x.CreatedByForeignKey == playerId);
            return Task.FromResult(questions);
        }

        public Task<IEnumerable<Question>> GetQuestionsInSession(Guid sessionId)
        {
            return Task.FromResult(_questions[sessionId] as IEnumerable<Question>);
        }

        public Task Submit(IEnumerable<Question> questions)
        {
            var allQuestions = _questions.Values.SelectMany(x => x).ToDictionary(x => x.Id);
            foreach (var question in questions)
            {
                var q = allQuestions[question.Id];
                q.Submitted = true;
                q.Text = question.Text;
            }

            return Task.CompletedTask;
        }
    }
}
