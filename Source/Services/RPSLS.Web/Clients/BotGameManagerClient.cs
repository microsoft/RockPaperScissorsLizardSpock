using GameApi.Proto;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RPSLS.Web.Config;
using RPSLS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Web.Clients
{
    public class BotGameManagerClient : BaseClient, IBotGameManagerClient
    {
        private readonly string _serverUrl;

        public BotGameManagerClient(IOptions<GameManagerSettings> settings, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _serverUrl = settings.Value.Url ?? throw new ArgumentNullException("Game Manager Url is null");
        }

        public async Task<ResultDto> Play(string challenger, string username, int pick, bool twitterLogged)
        {
            var request = new GameRequest()
            {
                Challenger = challenger,
                Username = username,
                TwitterLogged = twitterLogged,
                Pick = pick
            };

            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new BotGameManager.BotGameManagerClient(channel);
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
            var client = new BotGameManager.BotGameManagerClient(channel);
            var result = await client.GetChallengersAsync(new Empty(), GetRequestMetadata());
            return result.Challengers.Select(ChallengerDto.FromProtoResponse).ToList();
        }
    }
}
