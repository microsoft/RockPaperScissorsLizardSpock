using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RPSLS.Game.Client.Auth;
using RPSLS.Game.Client.Clients;
using RPSLS.Game.Client.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using GameBff.Proto;
using Blazor.Analytics;

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

            var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
            var grpcClient = GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress, new GrpcChannelOptions { HttpHandler = httpHandler });

            builder.Services.AddGrpcClient<BotGameManager.BotGameManagerClient>((services, options) =>
            {
                options.Address = new Uri(builder.HostEnvironment.BaseAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() => httpHandler);
            
            builder.Services.AddGrpcClient<MultiplayerGameManager.MultiplayerGameManagerClient>((services, options) =>
            {
                options.Address = new Uri(builder.HostEnvironment.BaseAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() => httpHandler);

            var gameSettingsManager = new GameSettingsManager.GameSettingsManagerClient(grpcClient);
            var settings = await gameSettingsManager.GetSettingsAsync(new Empty());

            builder.Services.AddSingleton(_ => settings.MultiplayerSettings);
            builder.Services.AddSingleton(_ => settings.TwitterSettings);
            builder.Services.AddSingleton(_ => settings.GoogleAnalyticsSettings);
            builder.Services.AddSingleton(_ => settings.RecognitionSettings);

            builder.Services.AddScoped<IBotGameManagerClient, BotGameManagerClient>();
            builder.Services.AddScoped<IBotGameService, BotGameService>();
            builder.Services.AddScoped<IMultiplayerGameManagerClient, MultiplayerGameManagerClient>();
            builder.Services.AddScoped<IMultiplayerGameService, MultiplayerGameService>();

            builder.Services.AddGoogleAnalytics();

            await builder.Build().RunAsync();
        }
    }
}
