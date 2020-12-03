using Blazor.Analytics;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPSLS.Game.Client.Auth;
using RPSLS.Game.Client.Clients;
using RPSLS.Game.Client.Extensions;
using RPSLS.Game.Client.Services;
using RPSLS.Game.Shared.Config;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RPSLS.Game.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

            var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            builder.Services.AddScoped(sp => httpClient);

            using var response = await httpClient.GetAsync("api/settings");
            using var stream = await response.Content.ReadAsStreamAsync();
            builder.Configuration.AddJsonStream(stream);

            builder.Services.AddSingleton(services =>
            {
                var httpHandler = new GrpcWebHandler(new HttpClientHandler());
                return GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress, new GrpcChannelOptions { HttpHandler = httpHandler });
            });

            builder.Services.AddSingleton(_ => builder.Configuration.GetSection(nameof(GameSettingsDto)).Get<GameSettingsDto>());
            builder.Services.AddSingleton(_ => builder.Configuration.GetSection(nameof(GameManagerSettings)).Get<GameManagerSettings>());
            builder.Services.AddSingleton(_ => builder.Configuration.GetSection(nameof(TwitterSettings)).Get<TwitterSettings>());
            builder.Services.AddSingleton(_ => builder.Configuration.GetSection(nameof(MultiplayerSettings)).Get<MultiplayerSettings>());

            builder.Services.AddScoped<IBotGameManagerClient, BotGameManagerClient>();
            //services.AddScoped<IMultiplayerGameManagerClient, MultiplayerGameManagerClient>();
            builder.Services.AddScoped<IBotGameService, BotGameService>();
            //services.AddScoped<IMultiplayerGameService, MultiplayerGameService>();

            var googleAnalyticsSettings = builder.Configuration.GetSection(nameof(GoogleAnalyticsSettings)).Get<GoogleAnalyticsSettings>();
            builder.Services.AddGoogleAnalyticsAndInitialize(googleAnalyticsSettings.GoogleAnalytics);

            await builder.Build().RunAsync();
        }
    }
}
