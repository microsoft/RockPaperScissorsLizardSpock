using Microsoft.Extensions.Options;
using RPSLS.Game.Shared.Config;

namespace RPSLS.Game.Server.Config
{
    public class ClientSettingsConfigureOptions : IConfigureOptions<ClientSettings>
    {
        private readonly IOptions<GoogleAnalyticsSettings> _googleAnalyticsSettings;
        private readonly IOptions<MultiplayerSettings> _multiplayerSettings;
        private readonly IOptions<RecognitionSettings> _recognitionSettings;
        private readonly IOptions<TwitterSettings> _twitterSettings;

        public ClientSettingsConfigureOptions(IOptions<GoogleAnalyticsSettings> googleAnalyticsSettings, IOptions<MultiplayerSettings> multiplayerSettings,
            IOptions<RecognitionSettings> recognitionSettings, IOptions<TwitterSettings> twitterSettings)
        {
            _googleAnalyticsSettings = googleAnalyticsSettings;
            _multiplayerSettings = multiplayerSettings;
            _recognitionSettings = recognitionSettings;
            _twitterSettings = twitterSettings;
        }


        public void Configure(ClientSettings options)
        {
            options.GoogleAnalyticsSettings = _googleAnalyticsSettings.Value;
            options.MultiplayerSettings = _multiplayerSettings.Value;
            options.RecognitionSettings = _recognitionSettings.Value;
            options.TwitterSettings = _twitterSettings.Value;
        }
    }
}
