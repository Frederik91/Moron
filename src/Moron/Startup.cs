using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moron.Server.Games.WhatIf.Answers;
using Moron.Server.Games.WhatIf.Games;
using Moron.Server.Games.WhatIf.Options;
using Moron.Server.Games.WhatIf.Questions;
using Moron.Server.Helpers;
using Moron.Server.Hubs;
using Moron.Server.Players;
using Moron.Server.SessionPlayers;
using Moron.Server.Sessions;
using System.Collections.Generic;

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

            services.AddSingleton<ISessionService, SessionService>();
            services.AddSingleton<IJoinIdGenerator, JoinIdGenerator>();
            services.AddSingleton<IPlayerService, PlayerService>();
            services.AddSingleton<ISessionPlayerService, SessionPlayerService>();
            services.AddSingleton<IWhatIfOptionService, WhatIfOptionService>();
            services.AddSingleton<IQuestionService, QuestionService>();
            services.AddSingleton<IAnswerService, AnswerService>();
            services.AddSingleton<IGameService, GameService>();
            services.AddSingleton<SessionHub>();
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
