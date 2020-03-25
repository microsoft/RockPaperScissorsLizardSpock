using GameApi.Proto;
using RPSLS.Game.Api.Config;
using System.Net.Http;

namespace RPSLS.Game.Api.Services
{
    public class ExternalChallenger : ChallengerBase, IChallenger
    {
        private readonly string _name;
        private readonly string _displayname;

        public ExternalChallenger(ChallengerOptions options, IHttpClientFactory httpClientFactory) : base(options.Url, httpClientFactory)
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
