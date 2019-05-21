using Moq;
using Moron.Server.Games.WhatIf.Answers;
using Moron.Server.Games.WhatIf.Games;
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
            var questionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var cut = new AnswerService();
            var answers = await cut.CreateAnswers(sessionId, questionIds);

            Assert.Equal(2, answers.Count());
        }
    }
}
