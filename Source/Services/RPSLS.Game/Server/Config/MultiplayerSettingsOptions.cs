using GameApi.Proto;
using Microsoft.Extensions.Options;


namespace RPSLS.Game.Server.Config
{
    public class MultiplayerSettingsOptions : IConfigureOptions<MultiplayerSettings>
    {
        private readonly ConfigurationManager.ConfigurationManagerClient _configurationManagerClient;

        public MultiplayerSettingsOptions(ConfigurationManager.ConfigurationManagerClient configurationManagerClient)
        {
            _configurationManagerClient = configurationManagerClient;
        }

        public async void Configure(MultiplayerSettings options)
        {
            var settingsResponse = await _configurationManagerClient.GetSettingsAsync(new Empty());
            options.Enabled = settingsResponse.HasMultiplayer;
        }
    }
}