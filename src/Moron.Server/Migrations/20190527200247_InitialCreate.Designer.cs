﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moron.Server.Contexts;

namespace Moron.Server.Migrations
{
    [DbContext(typeof(CommonContext))]
    [Migration("20190527200247_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("Moron.Server.Players.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Moron.Server.Sessions.PlayerSession", b =>
                {
                    b.Property<Guid>("SessionId");

                    b.Property<Guid>("PlayerId");

                    b.HasKey("SessionId", "PlayerId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerSessions");
                });

            modelBuilder.Entity("Moron.Server.Sessions.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("JoinId");

                    b.Property<string>("Name");

                    b.Property<Guid>("OwnerId");

                    b.Property<bool>("Started");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Moron.Server.Sessions.PlayerSession", b =>
                {
                    b.HasOne("Moron.Server.Players.Player", "Player")
                        .WithMany("SessionsLink")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Moron.Server.Sessions.Session", "Session")
                        .WithMany("PlayersLink")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}