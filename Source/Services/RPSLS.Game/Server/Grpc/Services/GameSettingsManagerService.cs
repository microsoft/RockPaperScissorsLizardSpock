using GameBff.Proto;
using Grpc.Core;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace RPSLS.Game.Server.Grpc.Services
{
    public class GameSettingsManagerService : GameSettingsManager.GameSettingsManagerBase
    {
        private readonly IOptions<Config.GoogleAnalyticsSettings> _googleAnalyticsSettings;
        private readonly IOptions<Config.RecognitionSettings> _recognitionSettings;
        private readonly IOptions<Config.TwitterSettings> _twitterSettings;
        private readonly IOptions<Config.MultiplayerSettings> _multiplayerSettings;

        public GameSettingsManagerService(
            IOptions<Config.GoogleAnalyticsSettings> googleAnalyticsSettings,
            IOptions<Config.RecognitionSettings> recognitionSettings,
            IOptions<Config.TwitterSettings> twitterSettings,
            IOptions<Config.MultiplayerSettings> multiplayerSettings)
        {
            _googleAnalyticsSettings = googleAnalyticsSettings;
            _recognitionSettings = recognitionSettings;
            _twitterSettings = twitterSettings;
            _multiplayerSettings = multiplayerSettings;
        }

        public override async Task<GameSettingsResponse> GetSettings(Empty request, ServerCallContext context)
        {
            var result = new GameSettingsResponse()
            {
                GoogleAnalyticsSettings = new GoogleAnalyticsSettings
                {
                    GoogleAnalytics = _googleAnalyticsSettings.Value.GoogleAnalytics,
                },
                MultiplayerSettings = new MultiplayerSettings()
                {
                    Enabled = _multiplayerSettings.Value.Enabled
                },
                RecognitionSettings = new RecognitionSettings()
                {
                    RecognitionThreshold = _recognitionSettings.Value.RecognitionThreshold
                },
                TwitterSettings = new TwitterSettings()
                {
                    IsLoginEnabled = _twitterSettings.Value.IsLoginEnabled,
                    AuthenticationScheme = _twitterSettings.Value.AuthenticationScheme
                }
            };

            return await Task.FromResult(result);
        }
    }
}
