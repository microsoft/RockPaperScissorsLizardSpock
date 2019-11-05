using GameApi.Proto;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RPSLS.Web.Config;
using RPSLS.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Web.Clients
{
    public class GameManagerClient : IGameManagerClient
    {
        private readonly string _serverUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameManagerClient(IOptions<GameManagerSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            _serverUrl = settings.Value.Url ?? throw new ArgumentNullException("Game Manager Url is null");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultDto> Play(string challenger, string username, int pick, bool twitterLogged)
        {
            var request = new GameRequest() {
                Challenger = challenger,
                Username = username,
                TwitterLogged = twitterLogged,
                Pick = pick
            };

            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new GameManager.GameManagerClient(channel);
            var result = await client.DoPlayAsync(request, GetRequestMetadata());
            return new ResultDto()
            {
                Challenger = result.Challenger,
                ChallengerPick = result.ChallengerPick,
                Result = (int)result.Result,
                User = result.User,
                UserPick = result.UserPick,
                IsValid = result.IsValid
            };
        }

        public async Task<IEnumerable<ChallengerDto>> Challengers()
        {
            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new GameManager.GameManagerClient(channel);
            var result = await client.GetChallengersAsync(new EmptyRequest(), GetRequestMetadata());
            return result.Challengers;
        }

        private Grpc.Core.Metadata GetRequestMetadata()
        {
            var metadata = new Grpc.Core.Metadata();
            var routeAs = _httpContextAccessor.HttpContext.Request.Headers["azds-route-as"].ToString();
            if (!String.IsNullOrEmpty(routeAs)) {
                metadata.Add("azds-route-as", routeAs);
            }
            return metadata;
        }
    }
}
