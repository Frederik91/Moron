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
                    var questionsAssignedToPlayer = questionsThisRound.Where(x => x.AssignedToPlayer == playerId);

                    var assignToPlayer = validPlayersToAssignTo.FirstOrDefault(x => !questionsAssignedToPlayer.Any() || questionsAssignedToPlayer.Any(y => y.CreatedBy != x));
                    if (assignToPlayer == Guid.Empty)
                    {
                        var questionToSwap = questionsThisRound.FirstOrDefault(x => x.AssignedToPlayer != playerId);

                        var questionsGroupsByAssigned = questionsThisRound.GroupBy(x => x.AssignedToPlayer);
                        var usersAssignedMoreThanOneQuestion = questionsGroupsByAssigned.Where(x => x.Count() > 1);
                        if (usersAssignedMoreThanOneQuestion.Any())
                        {
                            questionToSwap = usersAssignedMoreThanOneQuestion.First().First(x => x.CreatedBy != playerId);
                        }

                        if (questionToSwap is null)
                            assignToPlayer = playerIds.FirstOrDefault(x => x != playerId);
                        else
                        {
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

                    var a = questionsThisRound.GroupBy(x => x.AssignedToPlayer);
                    var b = a.Where(x => x.Count() > 1);
                    if (b.Any())
                    {
                        throw new Exception();
                    }
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
