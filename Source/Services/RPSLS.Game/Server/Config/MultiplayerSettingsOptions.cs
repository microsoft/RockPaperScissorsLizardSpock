using Microsoft.Extensions.Options;
using RPSLS.Game.Server.Clients;
using RPSLS.Game.Shared.Config;

namespace RPSLS.Game.Server.Config
{
    public class MultiplayerSettingsOptions : IConfigureOptions<MultiplayerSettings>
    {
        private readonly IConfigurationManagerClient _configurationManager;

        public MultiplayerSettingsOptions(IConfigurationManagerClient configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public void Configure(MultiplayerSettings options)
        {
            var gameApiSettingsClient = _configurationManager.GetSettings();
            options.Enabled = gameApiSettingsClient.HasMultiplayer;
        }
    }
}