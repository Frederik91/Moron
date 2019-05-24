using Moq;
using Moron.Server.Games.WhatIf.Answers;
using Moron.Server.Games.WhatIf.Games;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.SessionPlayers;
using Moron.Server.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TegGames.Server.Tests
{
    public class GameServiceTests
    {

        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        [InlineData(5, 1)]
        [InlineData(6, 1)]
        [InlineData(7, 1)]
        [InlineData(8, 1)]
        [InlineData(9, 1)]
        [InlineData(10, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(4, 2)]
        [InlineData(5, 2)]
        [InlineData(6, 2)]
        [InlineData(7, 2)]
        [InlineData(8, 2)]
        [InlineData(9, 2)]
        [InlineData(10, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(4, 3)]
        [InlineData(5, 3)]
        [InlineData(6, 3)]
        [InlineData(7, 3)]
        [InlineData(8, 3)]
        [InlineData(9, 3)]
        [InlineData(10, 3)]
        [InlineData(2, 4)]
        [InlineData(3, 4)]
        [InlineData(4, 4)]
        [InlineData(5, 4)]
        [InlineData(6, 4)]
        [InlineData(7, 4)]
        [InlineData(8, 4)]
        [InlineData(9, 4)]
        [InlineData(10, 4)]
        public async Task StartGame(int playerCount, int numberOfCards)
        {
            var sessionId = Guid.NewGuid();
            var players = new List<Guid>();
            for (var i = 0; i < playerCount; i++)
            {
                players.Add(Guid.NewGuid());
            }
            var questions = new List<Question>();
            for (var i = 0; i < players.Count * numberOfCards; i++)
            {
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = players[i%players.Count],
                    SessionId = sessionId,
                    Submitted = true,
                    Text = $"Q{i}"
                };
                questions.Add(question);
            }

            var answers = new List<Answer>();
            for (var i = 0; i < players.Count * numberOfCards; i++)
            {
                var createdBy = players[(i % players.Count + 1 % players.Count) == players.Count ? 0 : (i % players.Count + 1 % players.Count)]; 
                var answer = new Answer
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = createdBy,
                    QuestionId = questions[i%players.Count].Id,
                    SessionId = sessionId,
                    Submitted = true,
                    Text = $"A{i}"
                };
                answers.Add(answer);
            }

            var sessionPlayerServiceMock = new Mock<ISessionPlayerService>();
            sessionPlayerServiceMock.Setup(x => x.GetPlayersInSession(sessionId)).ReturnsAsync(players);
            var questionServiceMock = new Mock<IQuestionService>();
            questionServiceMock.Setup(x => x.GetQuestionsInSession(sessionId)).ReturnsAsync(questions);
            var answerServiceMock = new Mock<IAnswerService>();
            answerServiceMock.Setup(x => x.GetAnswersInSession(sessionId)).ReturnsAsync(answers);
            var optionsMock = new Mock<IWhatIfOptionService>();
            optionsMock.Setup(x => x.Get(sessionId)).ReturnsAsync(new WhatIfOption { Id = Guid.NewGuid(), NumberOfCards = numberOfCards });
            var cut = new GameService(sessionPlayerServiceMock.Object, questionServiceMock.Object, answerServiceMock.Object, optionsMock.Object);

            await cut.Start(sessionId);

            var turns = (await cut.GetTurns(sessionId)).ToList();

            Assert.Equal(questions.Count, turns.Count());
            Assert.True(turns.GroupBy(x => x.AnswerId).All(x => x.Count() == 1));
            Assert.True(turns.GroupBy(x => x.PlayerAnswerId).All(x => x.Count() == numberOfCards));
            Assert.True(turns.GroupBy(x => x.QuestionId).All(x => x.Count() == 1));
            Assert.True(turns.GroupBy(x => x.PlayerAnswerId).All(x => x.Count() == numberOfCards));

            var turn1 = turns.First();
            var turn1A = answers.First(x => x.Id == turn1.AnswerId);

            Assert.NotEqual(turn1.QuestionId, turn1A.QuestionId);
        }
    }
}
