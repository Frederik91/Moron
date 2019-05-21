using Moron.Server.Games.WhatIf.Answers;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.SessionPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Games
{
    public class GameService : IGameService
    {
        private static readonly Dictionary<Guid, Game> _games = new Dictionary<Guid, Game>();
        private readonly ISessionPlayerService _sessionPlayerService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly IWhatIfOptionService _whatIfOptionService;

        public GameService(ISessionPlayerService sessionPlayerService, IQuestionService questionService, IAnswerService answerService, IWhatIfOptionService whatIfOptionService)
        {
            _sessionPlayerService = sessionPlayerService;
            _questionService = questionService;
            _answerService = answerService;
            _whatIfOptionService = whatIfOptionService;
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
            if (game.IsFinished)
                return null;

            var turn = game.Turns.FirstOrDefault(x => !x.IsFinished);
            if (turn is null)
                game.IsFinished = true;
            return Task.FromResult(turn);
        }

        public Task SubmitTurn(Guid sessionId, Guid turnId)
        {
            var game = _games[sessionId];
            game.Turns.First(x => x.Id == turnId).IsFinished = true;

            return Task.CompletedTask;
        }

        public async Task Start(Guid sessionId)
        {
            var playerIds = await _sessionPlayerService.GetPlayersInSession(sessionId);
            var questions = await _questionService.GetQuestionsInSession(sessionId);
            var answers = await _answerService.GetAnswersInSession(sessionId);
            var options = await _whatIfOptionService.Get(sessionId);

            var turns = new List<Turn>();
            var assignedQuestions = new List<Guid>();
            foreach (var playerId in playerIds)
            {
                for (int i = 0; i < options.NumberOfCards; i++)
                {
                    var question = questions.First(x => !assignedQuestions.Contains(x.Id) && x.CreatedBy != playerId);
                    var turn = new Turn
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id,
                        PlayerQuestionId = playerId
                    };
                    turns.Add(turn);
                }
            }

            var assignedAnswers = new List<Guid>();
            foreach (var turn in turns)
            {
                var playerId = playerIds.First(x => x != turn.PlayerQuestionId && turns.Count(y => y.PlayerAnswerId == x) < options.NumberOfCards);
                var answer = answers.First(x => !assignedAnswers.Contains(x.Id) && x.CreatedBy != playerId && x.QuestionId != x.Id);
                turn.AnswerId = answer.Id;
                turn.PlayerAnswerId = playerId;
            }

            var game = new Game { Turns = turns };
            _games.Add(sessionId, game);
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
