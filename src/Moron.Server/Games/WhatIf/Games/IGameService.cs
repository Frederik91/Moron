using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Moron.Server.Games.WhatIf.Games
{
    public interface IGameService
    {
        Task<bool> GameIsStarted(Guid sessionId);
        Task<bool> GameIsFinished(Guid sessionId);
        Task<Turn> GetTurn(Guid sessionId);
        Task<IEnumerable<Turn>> GetTurns(Guid sessionId);
        Task Start(Guid sessionId);
        Task Next(Guid sessionId, Guid answerId);
    }
}
