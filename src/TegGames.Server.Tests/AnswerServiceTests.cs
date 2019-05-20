using Moron.Server.Games.WhatIf.Answers;
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
            var answer1 = await cut.GetRandomAnswer(sessionId, questionIds[0]);
            var answer2 = await cut.GetRandomAnswer(sessionId, questionIds[1]);

            Assert.Equal(2, answers.Count());
            Assert.NotEqual(answer1.Id, answer2.Id);
        }
    }
}
