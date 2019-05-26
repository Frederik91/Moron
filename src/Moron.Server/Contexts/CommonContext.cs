using Microsoft.EntityFrameworkCore;
using Moron.Server.Players;
using Moron.Server.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moron.Server.Contexts
{
    public class CommonContext : DbContext
    {
        public CommonContext(DbContextOptions<CommonContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerSession>()
                .HasOne(pt => pt.Player)
                .WithMany(p => p.SessionsLink)
                .HasForeignKey(pt => pt.PlayerId);

            modelBuilder.Entity<PlayerSession>()
                .HasOne(pt => pt.Session)
                .WithMany(t => t.PlayersLink)
                .HasForeignKey(pt => pt.SessionId);

            modelBuilder.Entity<PlayerSession>()
                .HasKey(x => new { x.SessionId, x.PlayerId });
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
