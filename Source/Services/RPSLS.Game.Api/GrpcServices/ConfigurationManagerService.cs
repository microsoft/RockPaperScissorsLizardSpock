using GameApi.Proto;
using Grpc.Core;
using RPSLS.Game.Multiplayer.Services;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.GrpcServices
{
    public class ConfigurationManagerService : ConfigurationManager.ConfigurationManagerBase
    {
        private readonly IPlayFabService _playFabService;

        public ConfigurationManagerService(IPlayFabService playFabService)
        {
            _playFabService = playFabService;
        }

        public override Task<SettingsResponse> GetSettings(Empty request, ServerCallContext context)
        {
            var response = new SettingsResponse()
            {
                HasMultiplayer = _playFabService.HasCredentials
            };

            return Task.FromResult(response);
        }
    }
}
