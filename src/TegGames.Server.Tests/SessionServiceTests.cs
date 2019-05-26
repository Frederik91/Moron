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
            var session = await cut.CreateAsync(name, player.PlayerId);

            Assert.NotNull(session);
            Assert.Equal(name, session.Name);
            Assert.Equal(player.PlayerId, session.OwnerId);
            Assert.Equal(player.PlayerId, session.Owner.PlayerId);
            Assert.Single(session.PlayersLink);
        }

        [Fact]
        public async Task CreateUser_CreateSession_JoineSessionNewUser()
        {
            var name = "Test";
            var joinIdGeneratorMock = new Mock<IJoinIdGenerator>();

            var playerService1 = new PlayerService(_context);
            var owner = await playerService1.Create("Owner");
            var cut1 = new SessionService(joinIdGeneratorMock.Object, _context);
            var session1 = await cut1.CreateAsync(name, owner.PlayerId);

            var context2 = CreateCommonContext(_connection);
            var playerService2 = new PlayerService(_context);
            var player = await playerService2.Create("Player");
            var cut2 = new SessionService(joinIdGeneratorMock.Object, context2);
            await cut2.AddPlayerToSession(session1.SessionId, player.PlayerId);

            session1 = await cut1.GetAsync(session1.SessionId);
            var session2 = await cut2.GetAsync(session1.SessionId);
            

            Assert.Equal(2, session2.PlayersLink.Count);
            Assert.True(session2.PlayersLink.TrueForAll(x => x.Player != null));
            Assert.True(session2.PlayersLink.TrueForAll(x => x.PlayerId != Guid.Empty));
            Assert.True(session2.PlayersLink.TrueForAll(x => x.Session != null));
            Assert.True(session2.PlayersLink.TrueForAll(x => x.SessionId != Guid.Empty));
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
