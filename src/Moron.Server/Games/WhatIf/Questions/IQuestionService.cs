﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Questions
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GenerateQuestionsForSession(Guid sessionId);
        Task Submit(IEnumerable<Question> questions);
        Task<bool> AllQuestionsSubmitted(Guid sessionId);

        Task<IEnumerable<Question>> GetQuestionsAssignedToPlayer(Guid sessionId, Guid playerId);
        Task<IEnumerable<Question>> GetQuestionsForPlayerToCreate(Guid sessionId, Guid playerId);
    }
}
