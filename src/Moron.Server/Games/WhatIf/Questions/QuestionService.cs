using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Players;
using Moron.Server.SessionPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Questions
{
    public class QuestionService : IQuestionService
    {
        private static readonly Dictionary<Guid, List<Question>> _questions = new Dictionary<Guid, List<Question>>();
        private readonly ISessionPlayerService _sessionPlayerService;
        private readonly IWhatIfOptionService _whatIfOptionService;

        public QuestionService(ISessionPlayerService sessionPlayerService, IWhatIfOptionService whatIfOptionService)
        {
            _sessionPlayerService = sessionPlayerService;
            _whatIfOptionService = whatIfOptionService;
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
            var playerIds = await _sessionPlayerService.GetPlayersInSession(sessionId);
            var questions = new List<Question>();
            foreach (var playerId in playerIds)
            {
                for (int i = 0; i < options.NumberOfCards; i++)
                {
                    var playersAssignedQuestions = questions.GroupBy(x => x.AssignedTo);
                    var playersAssignedAllQuestions = playersAssignedQuestions.Where(x => x.Count() == options.NumberOfCards).Select(x => x.Key);
                    var validPlayersToAssignTo = playerIds.Where(x => x != playerId && playersAssignedAllQuestions?.Contains(x) != true);
                    var assignToPlayer = validPlayersToAssignTo?.FirstOrDefault();

                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = playerId,
                        SessionId = sessionId,
                        AssignedTo = assignToPlayer.GetValueOrDefault(playerIds.First(x => x != playerId))
                    };
                    questions.Add(question);
                }
            }
            _questions.Add(sessionId, questions);
            return questions;
        }

        public Task<IEnumerable<Question>> GetQuestionsAssignedToPlayer(Guid sessionId, Guid playerId)
        {
            var questionsInSession = _questions[sessionId];
            var questions = questionsInSession.Where(x => x.AssignedTo == playerId);
            return Task.FromResult(questions);
        }

        public Task<IEnumerable<Question>> GetQuestionsForPlayerToCreate(Guid sessionId, Guid playerId)
        {
            var questionsInSession = _questions[sessionId];
            var questions = questionsInSession.Where(x => x.CreatedBy == playerId);
            return Task.FromResult(questions);
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
