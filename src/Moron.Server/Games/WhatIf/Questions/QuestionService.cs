using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Players;
using Moron.Server.SessionPlayers;
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
        private readonly ISessionPlayerService _sessionPlayerService;
        private readonly IWhatIfOptionService _whatIfOptionService;
        private readonly IPlayerService _playerService;

        public QuestionService(ISessionPlayerService sessionPlayerService, IWhatIfOptionService whatIfOptionService, IPlayerService playerService)
        {
            _sessionPlayerService = sessionPlayerService;
            _whatIfOptionService = whatIfOptionService;
            _playerService = playerService;
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
            var playerIds = (await _sessionPlayerService.GetPlayersInSession(sessionId)).ToList();
            var questions = new List<Question>();
            for (int i = 1; i <= options.NumberOfCards; i++)
            {
                var questionsThisRound = new List<Question>();
                foreach (var playerId in playerIds)
                {
                    var playersAssignedQuestions = questions.GroupBy(x => x.AssignedToPlayer);
                    var playersAssignedQuestionThisRound = questionsThisRound.Select(x => x.AssignedToPlayer);
                    var validPlayersToAssignTo = playerIds.Where(x => x != playerId && playersAssignedQuestionThisRound?.Contains(x) != true);

                    var assignToPlayer = validPlayersToAssignTo.FirstOrDefault();
                    if (assignToPlayer == Guid.Empty)
                    {
                        if (playerIds.Count == 2)
                            assignToPlayer = playerIds.FirstOrDefault(x => x != playerId);
                        else
                        {
                            var questionToSwap = questionsThisRound.FirstOrDefault();
                            assignToPlayer = questionToSwap.AssignedToPlayer;
                            questionToSwap.AssignedToPlayer = playerId;
                        }
                    }

                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = playerId,
                        SessionId = sessionId,
                        AssignedToPlayer = assignToPlayer
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
            var playerIds = unfinishedQuestions.Select(x => x.CreatedBy).Distinct();
            return _playerService.Get(playerIds);
        }

        public Task<IEnumerable<Question>> GetQuestionsAssignedToPlayer(Guid sessionId, Guid playerId)
        {
            var questionsInSession = _questions[sessionId];
            var questions = questionsInSession.Where(x => x.AssignedToPlayer == playerId);
            return Task.FromResult(questions);
        }

        public Task<IEnumerable<Question>> GetQuestionsCreatedByPlayer(Guid sessionId, Guid playerId)
        {
            var questionsInSession = _questions[sessionId];
            var questions = questionsInSession.Where(x => x.CreatedBy == playerId);
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
