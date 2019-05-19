using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Questions
{
    public class QuestionService : IQuestionService
    {
        private static readonly Dictionary<Guid, Question> _questions = new Dictionary<Guid, Question>();

        public Task<IEnumerable<Question>> CreateNQuestionsAsync(int amount)
        {
            var questions = new List<Question>();
            for (int i = 0; i < amount; i++)
            {
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    Text = "What happens if "
                };
                _questions.Add(question.Id, question);
                questions.Add(question);
            }
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
