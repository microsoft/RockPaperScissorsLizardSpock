using GameApi.Proto;
using k8s.Models;
using RPSLS.Game.Api.Config;
using System.Net.Http;

namespace RPSLS.Game.Api.Services
{
    public class InternalChallenger : ChallengerBase, IChallenger
    {
        private readonly string _name;
        private readonly string _displayname;

        public InternalChallenger(ChallengerOptions options, V1Service svc, IHttpClientFactory httpClientFactory) : base($"http://{svc.Metadata.Name}", httpClientFactory)
        {

            _name = options.Name;
            _displayname = options.DisplayName;
        }

        public ChallengerInfo Info
        {
            get => new ChallengerInfo
            {
                Name = _name,
                DisplayName = _displayname
            };
        }
    }
}
