using GameApi.Proto;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using RPSLS.Web.Config;
using RPSLS.Web.Models;
using System;

namespace RPSLS.Web.Clients
{
    public class ConfigurationManagerClient : IConfigurationManagerClient
    {
        private readonly string _serverUrl;

        public ConfigurationManagerClient(IOptions<GameManagerSettings> settings)
        {
            _serverUrl = settings.Value.Url ?? throw new ArgumentNullException("Game Manager Url is null");
        }

        public GameSettingsDto GetSettings()
        {
            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new ConfigurationManager.ConfigurationManagerClient(channel);
            var result = client.GetSettings(new Empty());
            return new GameSettingsDto
            {
                HasMultiplayer = result.HasMultiplayer
            };
        }
    }
}
