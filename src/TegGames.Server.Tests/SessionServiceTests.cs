using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moron.Server.Contexts;
using Moron.Server.Helpers;
using Moron.Server.Players;
using Moron.Server.Sessions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TegGames.Server.Tests
{
    public class SessionServiceTests
    {
        private readonly SqliteConnection _connection;
        private readonly CommonContext _context;

        public SessionServiceTests()
        {
            _connection = CreateConnection();
            _context = CreateCommonContext(_connection);
        }

        [Fact]
        public async Task CreateUser_CreateSession_Verify_Created_And_Joined()
        {
            var name = "Test";
            var joinIdGeneratorMock = new Mock<IJoinIdGenerator>();

            var playerService = new PlayerService(_context);
            var player = await playerService.Create("Test player");
            var cut = new SessionService(joinIdGeneratorMock.Object, _context);
            var session = await cut.CreateAsync(name, player.Id);

            Assert.NotNull(session);
            Assert.Equal(name, session.Name);
            Assert.Equal(player.Id, session.OwnerId);
            Assert.Single(session.PlayersLink);
        }

        [Theory]
        [InlineData(10)]
        public async Task CreateUser_CreateSession_JoineSessionNewUser(int playerCount)
        {
            var name = "Test";
            var joinIdGeneratorMock = new Mock<IJoinIdGenerator>();

            var cut = new SessionService(joinIdGeneratorMock.Object, _context);
            var sessionId = Guid.NewGuid();
            for (int i = 0; i < playerCount; i++)
            {
                var context = CreateCommonContext(_connection);
                var playerService = new PlayerService(context);
                var player = await playerService.Create("Player " + i + 1);
                var tempCut = new SessionService(joinIdGeneratorMock.Object, context);
                if (i == 0)
                {
                   var s = await tempCut.CreateAsync(name, player.Id);
                    sessionId = s.Id;
                }
                else
                {
                    await tempCut.AddPlayerToSession(sessionId, player.Id);
                }
            }

            var session = await cut.GetAsync(sessionId);




            Assert.Equal(playerCount, session.PlayersLink.Count);
            Assert.True(session.PlayersLink.TrueForAll(x => x.Player != null));
            Assert.True(session.PlayersLink.TrueForAll(x => x.PlayerId != Guid.Empty));
            Assert.True(session.PlayersLink.TrueForAll(x => x.Session != null));
            Assert.True(session.PlayersLink.TrueForAll(x => x.SessionId != Guid.Empty));
        }

        private static CommonContext CreateCommonContext(SqliteConnection connection)
        {
            var builder = new DbContextOptionsBuilder<CommonContext>();
            builder.UseSqlite(connection);
            var context = new CommonContext(builder.Options);
            return context;
        }

        private static SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }
    }
}
