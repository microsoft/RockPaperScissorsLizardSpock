using GameApi.Proto;
using RPSLS.Game.Server.Config;

namespace RPSLS.Game.Server.Clients
{
    public class ConfigurationManagerClient : IConfigurationManagerClient
    {
        private readonly ConfigurationManager.ConfigurationManagerClient _configurationManagerClient;

        public ConfigurationManagerClient(ConfigurationManager.ConfigurationManagerClient configurationManagerClient)
        {
            _configurationManagerClient = configurationManagerClient;
        }

        public GameSettingsDto GetSettings()
        {
            var result = _configurationManagerClient.GetSettings(new Empty());
            return new GameSettingsDto
            {
                HasMultiplayer = result.HasMultiplayer
            };
        }
    }
}
