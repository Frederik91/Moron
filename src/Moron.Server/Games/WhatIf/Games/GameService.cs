using Moron.Server.Games.WhatIf.Answers;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.Sessions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Games
{
    public class GameService : IGameService
    {
        private static readonly ConcurrentDictionary<Guid, Game> _games = new ConcurrentDictionary<Guid, Game>();
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly IWhatIfOptionService _whatIfOptionService;
        private readonly ISessionService _sessionService;

        public GameService(IQuestionService questionService, IAnswerService answerService, IWhatIfOptionService whatIfOptionService, ISessionService sessionService)
        {
            _questionService = questionService;
            _answerService = answerService;
            _whatIfOptionService = whatIfOptionService;
            _sessionService = sessionService;
        }

        public Task<IEnumerable<Turn>> GetTurns(Guid sessionId)
        {
            var game = _games[sessionId];
            return Task.FromResult(game.Turns as IEnumerable<Turn>);
        }

        public Task<bool> GameIsFinished(Guid sessionId)
        {
            return Task.FromResult(_games[sessionId].IsFinished);
        }

        public Task<Turn> GetTurn(Guid sessionId)
        {
            var game = _games[sessionId];
            var turn = game.Turns.FirstOrDefault(x => !x.IsFinished);
            if (turn is null)
                game.IsFinished = true;
            return Task.FromResult(turn);
        }

        public async Task Start(Guid sessionId)
        {
            var playerIds = (await _sessionService.GetPlayersInSession(sessionId)).OrderBy(x => x).Select(x => x.PlayerId).ToList();
            var questions = (await _questionService.GetQuestionsInSession(sessionId)).OrderBy(x => x.CreatedByForeignKey).ToList();
            var answers = (await _answerService.GetAnswersInSession(sessionId)).OrderBy(x => x.CreatedByForeignKey).ToList();
            var options = await _whatIfOptionService.Get(sessionId);

            var turns = new List<Turn>();
            var assignedQuestions = new List<Guid>();
            for (var n = 0; n < playerIds.Count; n++)
            {
                var playerId = playerIds[n];
                var nextPlayer = playerIds[(n % playerIds.Count + 1 % playerIds.Count) == playerIds.Count ? 0 : (n % playerIds.Count + 1 % playerIds.Count)];
                var questionsByNextPlayer = questions.Where(x => x.CreatedByForeignKey == nextPlayer).ToList();
                var answersByNextPlayer = answers.Where(x => x.CreatedByForeignKey == nextPlayer).ToList();

                for (var i = 0; i < options.NumberOfCards; i++)
                {
                    var question = questionsByNextPlayer[i];
                    var answer = answersByNextPlayer[i];
                    var turn = new Turn
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id,
                        PlayerQuestionId = playerId,
                        AnswerId = answer.Id,
                        PlayerAnswerId = playerIds.Count == 2 ? nextPlayer : playerIds.Where(x => turns.Count(y => y.PlayerAnswerId == x) < options.NumberOfCards).FirstOrDefault(x => x != playerId && x != nextPlayer)
                    };
                    assignedQuestions.Add(question.Id);
                    turns.Add(turn);
                }
            }

            var rng = new Random(DateTime.Now.Millisecond);

            void Shuffle<T>(IList<T> list)
            {
                var n = list.Count;
                while (n > 1)
                {
                    n--;
                    var k = rng.Next(n + 1);
                    var value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }

            Shuffle(turns);

            var game = new Game { Turns = turns };
            _games.TryAdd(sessionId, game);
        }

        public Task<bool> GameIsStarted(Guid sessionId)
        {
            var result = _games.ContainsKey(sessionId);
            return Task.FromResult(result);
        }

        public Task Next(Guid sessionId, Guid answerId)
        {
            var game = _games[sessionId];
            game.Turns.First(x => x.AnswerId == answerId).IsFinished = true;
            return Task.CompletedTask;
        }
    }
}
