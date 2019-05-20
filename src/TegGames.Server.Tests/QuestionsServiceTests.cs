using Moq;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.SessionPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TegGames.Server.Tests
{
    public class QuestionsServiceTests
    {
        [Fact]
        public async Task GenerateQuestionsForSession_GenerateQuestions()
        {
            var sessionId = Guid.NewGuid();
            var playersInSesssion = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var options = new WhatIfOption { Id = Guid.NewGuid(), NumberOfCards = 3 };

            var sessionPlayerServiceMock = new Mock<ISessionPlayerService>();
            sessionPlayerServiceMock.Setup(x => x.GetPlayersInSession(sessionId)).ReturnsAsync(playersInSesssion);
            var whatIfOptionServiceMock = new Mock<IWhatIfOptionService>();
            whatIfOptionServiceMock.Setup(x => x.Get(sessionId)).ReturnsAsync(options);

            var cut = new QuestionService(sessionPlayerServiceMock.Object, whatIfOptionServiceMock.Object);
            var questions = await cut.GenerateQuestionsForSession(sessionId);

            foreach (var player in playersInSesssion)
            {
                var questionsAssignedPlayerCreate = await cut.GetQuestionsForPlayerToCreate(sessionId, player);
                Assert.Equal(options.NumberOfCards, questionsAssignedPlayerCreate.Count());

                var questionsAssignedPlayerAnswer = await cut.GetQuestionsAssignedToPlayer(sessionId, player);
                Assert.Equal(options.NumberOfCards, questionsAssignedPlayerAnswer.Count());
            }
        }
    }
}
