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

        [Fact]
        public async Task StartGame()
        {
            var sessionId = Guid.NewGuid();
            var players = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var questions = new List<Question>
            {
                new Question
                {
                    Id = Guid.NewGuid(),
                    AssignedTo = players[1],
                    CreatedBy = players[0],
                    SessionId = sessionId,
                    Submitted = true,
                    Text = "Q1"
                },
                new Question
                {
                    Id = Guid.NewGuid(),
                    AssignedTo = players[0],
                    CreatedBy = players[1],
                    SessionId = sessionId,
                    Submitted = true,
                    Text = "Q2"
                }
            };
            var answers = new List<Answer>
            {
                new Answer
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = players[1],
                    QuestionId = questions[0].Id,
                    SessionId = sessionId,
                    Submitted = true,
                    Text = "A1"
                },
                new Answer
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = players[0],
                    QuestionId = questions[1].Id,
                    SessionId = sessionId,
                    Submitted = true,
                    Text = "A1"
                }
            };

            var sessionPlayerServiceMock = new Mock<ISessionPlayerService>();
            sessionPlayerServiceMock.Setup(x => x.GetPlayersInSession(sessionId)).ReturnsAsync(players);
            var questionServiceMock = new Mock<IQuestionService>();
            questionServiceMock.Setup(x => x.GetQuestionsInSession(sessionId)).ReturnsAsync(questions);
            var answerServiceMock = new Mock<IAnswerService>();
            answerServiceMock.Setup(x => x.GetAnswersInSession(sessionId)).ReturnsAsync(answers);
            var optionsMock = new Mock<IWhatIfOptionService>();
            optionsMock.Setup(x => x.Get(sessionId)).ReturnsAsync(new WhatIfOption { Id = Guid.NewGuid(), NumberOfCards = 1 });
            var cut = new GameService(sessionPlayerServiceMock.Object, questionServiceMock.Object, answerServiceMock.Object, optionsMock.Object);

            await cut.Start(sessionId);

            var turns = await cut.GetTurns(sessionId);

            Assert.Equal(questions.Count, turns.Count());
            Assert.True(turns.GroupBy(x => x.AnswerId).All(x => x.Count() == 1));
            Assert.True(turns.GroupBy(x => x.PlayerAnswerId).All(x => x.Count() == 1));
            Assert.True(turns.GroupBy(x => x.QuestionId).All(x => x.Count() == 1));
            Assert.True(turns.GroupBy(x => x.PlayerAnswerId).All(x => x.Count() == 1));
        }
    }
}
