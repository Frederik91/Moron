using Moron.Server.Games.WhatIf.QuestionAnswers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Questions
{
    public class QuestionService : IQuestionService
    {
        private static readonly Dictionary<Guid, Question> _questions = new Dictionary<Guid, Question>();
        private readonly IQuestionAnswerService _questionAnswerService;

        public QuestionService(IQuestionAnswerService questionAnswerService)
        {
            _questionAnswerService = questionAnswerService;
        }

        public async Task<bool> AllQuestionsCreated(Guid sessionId)
        {
            var questionIds = await _questionAnswerService.GetQuestionsInSession(sessionId);
            var result = _questions.Values.Where(x => questionIds.Any(y => y.QuestionId == x.Id) && !x.Submitted).Any();
            return !result;
        }

        public Task<IEnumerable<Question>> CreateNQuestionsAsync(Guid sessionId, int amount)
        {
            var questions = new List<Question>();
            for (int i = 0; i < amount; i++)
            {
                var question = new Question
                {
                    Id = Guid.NewGuid()
                };
                _questions.Add(question.Id, question);
                questions.Add(question);
            }
            _questionAnswerService.AddQuestionsToSession(sessionId, questions.Select(x => x.Id));
            return Task.FromResult(questions as IEnumerable<Question>);
        }

        public Task Update(IEnumerable<Question> questions)
        {
            foreach (var question in questions)
            {
                _questions[question.Id] = question;
            }

            return Task.CompletedTask;
        }
    }
}
