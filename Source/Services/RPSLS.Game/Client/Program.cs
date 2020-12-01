using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using RPSLS.Game.Client.Auth;
using RPSLS.Game.Client.Helpers;
using RPSLS.Game.Shared.Config;
using Microsoft.Extensions.Configuration;

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
            builder.Services.AddScoped<SvgHelper>();

            builder.Services.AddOptions();

            using var response = await httpClient.GetAsync("api/settings");
            using var stream = await response.Content.ReadAsStreamAsync();

            builder.Configuration.AddJsonStream(stream);

            builder.Services.AddSingleton(_ => builder.Configuration.GetSection(nameof(GameSettingsDto)).Get<GameSettingsDto>());

            await builder.Build().RunAsync();
    }
}
}
