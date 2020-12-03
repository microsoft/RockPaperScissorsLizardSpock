using GameBff.Proto;
using Grpc.Core;
using Microsoft.Extensions.Options;
using RPSLS.Game.Server.Config;
using System.Threading.Tasks;

namespace RPSLS.Game.Server.GrpcServices
{
    public class GameSettingsManagerService : GameSettingsManager.GameSettingsManagerBase
    {
        private readonly IOptions<ClientSettings> _clientSettings;

        public GameSettingsManagerService(IOptions<ClientSettings> clientSettings)
        {
            _clientSettings = clientSettings;
        }

        public override async Task<GameSettingsResponse> GetSettings(GameBff.Proto.Empty request, ServerCallContext context)
        {
            var clientSettings = _clientSettings.Value;

            var result = new GameSettingsResponse()
            {
                GoogleAnalyticsSettings = new GameBff.Proto.GoogleAnalyticsSettings
                {
                    GoogleAnalytics = clientSettings.GoogleAnalyticsSettings.GoogleAnalytics,
                },
                MultiplayerSettings = new GameBff.Proto.MultiplayerSettings()
                {
                    Enabled = clientSettings.MultiplayerSettings.Enabled
                },
                RecognitionSettings = new GameBff.Proto.RecognitionSettings()
                {
                    RecognitionThreshold = clientSettings.RecognitionSettings.RecognitionThreshold
                },
                TwitterSettings = new GameBff.Proto.TwitterSettings()
                {
                    IsLoginEnabled = clientSettings.TwitterSettings.IsLoginEnabled,
                    AuthenticationScheme = clientSettings.TwitterSettings.AuthenticationScheme
                }
            };

            return await Task.FromResult(result);
        }
    }
}
