using Microsoft.Extensions.Options;
using RPSLS.Game.Client.Clients;

namespace RPSLS.Game.Client.Config
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

    public class MultiplayerSettings 
    {

        public bool Enabled { get; set; }
    }
}