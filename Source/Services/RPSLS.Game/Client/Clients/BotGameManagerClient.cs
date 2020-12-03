using GameBff.Proto;
using Grpc.Net.Client;
using RPSLS.Game.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Client.Clients
{
    public class BotGameManagerClient : IBotGameManagerClient
    {
        private readonly GrpcChannel _grpcChannel;

        public BotGameManagerClient(GrpcChannel GrpcChannel)
        {
            _grpcChannel = GrpcChannel;
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

            var client = new BotGameManager.BotGameManagerClient(_grpcChannel);
            var result = await client.DoPlayAsync(request);
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
            var client = new BotGameManager.BotGameManagerClient(_grpcChannel);
            var result = await client.GetChallengersAsync(new Empty());
            return result.Challengers.Select(ChallengerDto.FromProtoResponse).ToList();
        }
    }
}
