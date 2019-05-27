using Moq;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.Players;
using Moron.Server.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TegGames.Server.Tests
{
    public class QuestionsServiceTests
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
        public async Task GenerateQuestionsForSession_GenerateQuestions(int playerCount, int numberOfCards)
        {
            var sessionId = Guid.NewGuid();
            var playersInSesssion = new List<Player>();
            for (int i = 0; i < playerCount; i++)
            {
                playersInSesssion.Add(new Player
                {
                    Id = Guid.NewGuid()
                });
            }
            var options = new WhatIfOption { Id = Guid.NewGuid(), NumberOfCards = numberOfCards };

            var sessionServiceMock = new Mock<ISessionService>();
            sessionServiceMock.Setup(x => x.GetPlayersInSession(sessionId)).ReturnsAsync(playersInSesssion);
            var whatIfOptionServiceMock = new Mock<IWhatIfOptionService>();
            whatIfOptionServiceMock.Setup(x => x.Get(sessionId)).ReturnsAsync(options);

            var playerServiceMock = new Mock<IPlayerService>();

            var cut = new QuestionService(whatIfOptionServiceMock.Object, playerServiceMock.Object, sessionServiceMock.Object);
            var questions = await cut.GenerateQuestionsForSession(sessionId);

            foreach (var player in playersInSesssion)
            {
                var questionsCreatedByPlayer = (await cut.GetQuestionsCreatedByPlayer(sessionId, player.Id)).ToList();
                Assert.Equal(options.NumberOfCards, questionsCreatedByPlayer.Count);

                var questionsAssignedPlayerAnswer = (await cut.GetQuestionsAssignedToPlayer(sessionId, player.Id)).ToList();
                Assert.Equal(options.NumberOfCards, questionsAssignedPlayerAnswer.Count);
            }
        }
    }
}
