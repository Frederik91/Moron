using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moron.Server.Contexts;
using Moron.Server.Games.WhatIf.Answers;
using Moron.Server.Games.WhatIf.Games;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.Helpers;
using Moron.Server.Hubs;
using Moron.Server.Players;
using Moron.Server.Sessions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;

namespace Moron
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddServerSideBlazor();
            //.AddSignalR().AddAzureSignalR(Configuration["Azure:SignalR:ConnectionString"]);

            services.AddScoped<ISessionService, SessionService>();
            services.AddSingleton<IJoinIdGenerator, JoinIdGenerator>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IWhatIfOptionService, WhatIfOptionService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IGameService, GameService>();
            services.AddSingleton<SessionHub>();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            SQLiteConnection.CreateFile("TegGamesDb.sqlite");
            services.AddDbContext<CommonContext>(opt => opt.UseSqlite(connectionString)) ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAzureSignalR(endpoints =>
            //{
            //    endpoints.MapHub<SessionHub>("/session");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
