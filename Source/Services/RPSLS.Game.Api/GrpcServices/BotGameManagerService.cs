using GameApi.Proto;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using RPSLS.Game.Api.Data;
using RPSLS.Game.Api.Services;
using RPSLS.Game.Multiplayer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.GrpcServices
{
    public class BotGameManagerService : BotGameManager.BotGameManagerBase
    {
        private readonly IPlayFabService _playFabService;
        private readonly IChallengerService _challengersService;
        private readonly IGameService _gameService;
        private readonly IMatchesRepository _resultsDao;
        private readonly ILogger<BotGameManagerService> _logger;

        public BotGameManagerService(
            IPlayFabService playFabService,
            IChallengerService challengers,
            IGameService gameService,
            IMatchesRepository resultsDao,
            ILogger<BotGameManagerService> logger
            )
        {
            _playFabService = playFabService;
            _challengersService = challengers;
            _gameService = gameService;
            _resultsDao = resultsDao;
            _logger = logger;
        }

        public override Task<ChallengersList> GetChallengers(Empty request, ServerCallContext context)
        {
            var result = new ChallengersList();
            foreach (var challenger in _challengersService.Challengers)
            {
                result.Challengers.Add(challenger.Info);
            }

            result.Count = result.Challengers.Count;
            return Task.FromResult(result);
        }

        public override async Task<GameResponse> DoPlay(GameRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Player {request.Username} picked {PickDto.ToText(request.Pick)} against {request.Challenger}.");

            var challenger = _challengersService.SelectChallenger(request);

            var result = new GameResponse()
            {
                Challenger = request.Challenger,
                User = request.Username,
                UserPick = request.Pick
            };

            var pick = await challenger.Pick(GetContext(context), request.TwitterLogged, request.Username);
            _logger.LogInformation($"Challenger {result.Challenger} picked {PickDto.ToText(pick.Value)} against {request.Username}.");

            result.ChallengerPick = pick.Value;
            result.IsValid = IsValid(pick);
            result.Result = result.IsValid ? _gameService.Check(result.UserPick, result.ChallengerPick) : Result.Player;
            _logger.LogInformation($"Result of User {request.Username} vs Challenger {result.Challenger}, winner: {result.Result}");

            if (result.IsValid && request.TwitterLogged)
            {
                await _resultsDao.SaveMatch(pick, request.Username, request.Pick, result.Result);
            }

            if (_playFabService.HasCredentials)
            {
                var username = !request.TwitterLogged ? $"${request.Username}" : request.Username;
                await _playFabService.UpdateStats(username, result.Result == Result.Player);
            }

            return result;
        }

        private IDictionary<string, string> GetContext(ServerCallContext context)
        {
            return context.RequestHeaders
                .Where(e => e.Key == "azds-route-as")
                .ToDictionary(e => e.Key, e => e.Value);
        }

        private static bool IsValid(PickDto pick)
        {
            if (pick.Value > 4 || pick.Value < 0)
                return false;

            if (pick.Text.ToLowerInvariant() != PickDto.ToText(pick.Value).ToLowerInvariant())
                return false;
            
            return true;
        }
    }
}
