using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RPSLS.Game.Api.Config;
using RPSLS.Game.Api.Data;
using RPSLS.Game.Api.GrpcServices;
using RPSLS.Game.Api.Services;
using System.Collections.Generic;
using System.Net.Http;

namespace RPSLS.Game.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddScoped<ResultsDao>(sp => new ResultsDao(Configuration["cosmos-constr"], sp.GetService<ILoggerFactory>()));
            services.AddTransient<IGameService, GameService>();
            services.AddHttpClient("Challenger");
            services.AddGrpc();

            var challengers = new List<ChallengerOptions>();
            Configuration.Bind("Challengers", challengers);
            services.AddSingleton<IChallengerService>(sp => new ChallengerService(
                    sp.GetService<IHttpClientFactory>(),
                    challengers,
                    sp.GetService<ILoggerFactory>()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GameManagerService>();
                endpoints.MapControllers();
            });
        }
    }
}
