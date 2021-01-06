using GameBff.Proto;
using RPSLS.Game.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Client.Clients
{
    public class BotGameManagerClient : IBotGameManagerClient
    {
        private readonly BotGameManager.BotGameManagerClient _botGameManagerClient;

        public BotGameManagerClient(BotGameManager.BotGameManagerClient botGameManagerClient)
        {
            _botGameManagerClient = botGameManagerClient;
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

            var result = await _botGameManagerClient.DoPlayAsync(request);
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
            var result = await _botGameManagerClient.GetChallengersAsync(new Empty());
            return result.Challengers.Select(ChallengerDto.FromProtoResponse).ToList();
        }
    }
}
