using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RPSLS.Game.Multiplayer.Config;
using RPSLS.Game.Multiplayer.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MultiplayerExtensions
    {
        public static void AddMultiplayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MultiplayerSettings>(configuration.GetSection("Multiplayer"));
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<IPlayFabService, PlayFabService>();
        }

        public static void UsePlayFab(this IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            var playFabService = app.ApplicationServices.GetService<IPlayFabService>();
            applicationLifetime.ApplicationStarted.Register(async () => await playFabService.Initialize());
        }
    } 
}
