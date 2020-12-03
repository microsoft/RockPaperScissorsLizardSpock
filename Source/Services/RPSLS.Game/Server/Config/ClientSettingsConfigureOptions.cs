using Microsoft.Extensions.Options;

namespace RPSLS.Game.Server.Config
{
    public class ClientSettingsConfigureOptions : IConfigureOptions<ClientSettings>
    {
        private readonly IOptions<GoogleAnalyticsSettings> _googleAnalyticsSettings;
        private readonly IOptions<MultiplayerSettings> _multiplayerSettings;
        private readonly IOptions<RecognitionSettings> _recognitionSettings;
        private readonly IOptions<TwitterSettings> _twitterSettings;
        private readonly IOptions<GameManagerSettings> _gameManagerSettings;

        public ClientSettingsConfigureOptions(IOptions<GoogleAnalyticsSettings> googleAnalyticsSettings, 
            IOptions<RecognitionSettings> recognitionSettings, IOptions<TwitterSettings> twitterSettings,
            IOptions<GameManagerSettings> gameManagerSettings, IOptions<MultiplayerSettings> multiplayerSettings)
        {
            _googleAnalyticsSettings = googleAnalyticsSettings;
            _recognitionSettings = recognitionSettings;
            _twitterSettings = twitterSettings;
            _gameManagerSettings = gameManagerSettings;
            _multiplayerSettings = multiplayerSettings;
        }


        public void Configure(ClientSettings options)
        {
            options.GoogleAnalyticsSettings = _googleAnalyticsSettings.Value;
            options.MultiplayerSettings = _multiplayerSettings.Value;
            options.RecognitionSettings = _recognitionSettings.Value;
            options.TwitterSettings = _twitterSettings.Value;
            options.GameManagerSettings = _gameManagerSettings.Value;
        }
    }
}
