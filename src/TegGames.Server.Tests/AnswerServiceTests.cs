﻿using Moq;
using Moron.Server.Games.WhatIf.Answers;
using Moron.Server.Games.WhatIf.Games;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TegGames.Server.Tests
{
    public class AnswerServiceTests
    {
        [Fact]
        public async Task GetAnswers()
        {
            var sessionId = Guid.NewGuid();
            var questionIds = new List<Question> { new Question { Id = Guid.NewGuid() }, new Question { Id = Guid.NewGuid() } };

            var playerServiceMock = new Mock<IPlayerService>();

            var cut = new AnswerService(playerServiceMock.Object);
            var answers = await cut.GenerateSessionAnswers(sessionId, questionIds);

            Assert.Equal(2, answers.Count());
        }
    }
}
