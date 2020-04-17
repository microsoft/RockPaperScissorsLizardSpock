using GameApi.Proto;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RPSLS.Game.Api.Data;
using RPSLS.Game.Api.Data.Models;
using RPSLS.Game.Api.Services;
using RPSLS.Game.Multiplayer.Config;
using RPSLS.Game.Multiplayer.Services;
using System;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.GrpcServices
{
    public class MultiplayerGameManagerService : MultiplayerGameManager.MultiplayerGameManagerBase
    {
        private const int FREE_TIER_MAX_REQUESTS = 10;
        private const string UnknownUser = "-";
        private readonly GameStatusResponse _cancelledMatch = new GameStatusResponse { IsCancelled = true };

        private readonly IPlayFabService _playFabService;
        private readonly ITokenService _tokenService;
        private readonly IGameService _gameService;
        private readonly IMatchesRepository _repository;
        private readonly MultiplayerSettings _multiplayerSettings;
        private readonly ILogger<MultiplayerGameManagerService> _logger;

        public MultiplayerGameManagerService(
            IPlayFabService playFabService,
            ITokenService tokenService,
            IGameService gameService,
            IOptions<MultiplayerSettings> options,
            IMatchesRepository repository,
            ILogger<MultiplayerGameManagerService> logger)
        {
            _playFabService = playFabService;
            _tokenService = tokenService;
            _gameService = gameService;
            _repository = repository;
            _multiplayerSettings = options.Value;
            _logger = logger;

            if (_multiplayerSettings.Token.TicketStatusWait < 60000 / FREE_TIER_MAX_REQUESTS)
            {
                _logger.LogWarning($"PlayFab free tier limits the Get Matchmaking Ticket requests to a max of {FREE_TIER_MAX_REQUESTS} per minute. " +
                    "A MatchmakingRateLimitExceeded error might occur while waiting for a multiplayer match");
            }
        }

        public override async Task CreatePairing(CreatePairingRequest request, IServerStreamWriter<PairingStatusResponse> responseStream, ServerCallContext context)
        {
            var token = _tokenService.GenerateToken();
            try
            {
                var username = GetUsername(request.Username, request.TwitterLogged);
                var ticketId = await _playFabService.CreateTicket(username, token);
                _logger.LogInformation($"New token created for user {username}: {token}");
                await responseStream.WriteAsync(CreateMatchStatusResponse("TokenCreated", token));

                await Task.Delay(_multiplayerSettings.Token.TicketStatusWait);
                var matchResult = await _playFabService.CheckTicketStatus(username, ticketId);
                while (!matchResult.Finished && !context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(CreateMatchStatusResponse(matchResult.Status, token));
                    await Task.Delay(_multiplayerSettings.Token.TicketStatusWait);
                    matchResult = await _playFabService.CheckTicketStatus(username, ticketId);
                }

                await responseStream.WriteAsync(CreateMatchStatusResponse(matchResult.Status, token, matchResult.MatchId));
                await _repository.CreateMatch(matchResult.MatchId, username, matchResult.Opponent);
            }
            catch
            {
                await responseStream.WriteAsync(CreateMatchStatusResponse("Canceled", token));
            }
        }

        public override async Task JoinPairing(JoinPairingRequest request, IServerStreamWriter<PairingStatusResponse> responseStream, ServerCallContext context)
        {
            try
            {
                var username = GetUsername(request.Username, request.TwitterLogged);
                var ticketId = await _playFabService.CreateTicket(username, request.Token);
                await Task.Delay(_multiplayerSettings.Token.TicketStatusWait);
                var matchResult = await _playFabService.CheckTicketStatus(username, ticketId);
                while (!matchResult.Finished && !context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(CreateMatchStatusResponse(matchResult.Status, request.Token));
                    await Task.Delay(_multiplayerSettings.Token.TicketStatusWait);
                    matchResult = await _playFabService.CheckTicketStatus(username, ticketId);
                }

                await responseStream.WriteAsync(CreateMatchStatusResponse(matchResult.Status, request.Token, matchResult.MatchId));
            }
            catch
            {
                await responseStream.WriteAsync(CreateMatchStatusResponse("Canceled", request.Token));
            }
        }

        public override async Task GameStatus(GameStatusRequest request, IServerStreamWriter<GameStatusResponse> responseStream, ServerCallContext context)
        {
            var username = GetUsername(request.Username, request.TwitterLogged);
            var dto = await _repository.GetMatch(request.MatchId);
            while (!context.CancellationToken.IsCancellationRequested && (dto == null || (dto.PlayerName == UnknownUser && dto.Challenger.Name == UnknownUser)))
            {
                await Task.Delay(_multiplayerSettings.GameStatusUpdateDelay);
                dto = await _repository.GetMatch(request.MatchId);
            }

            var isMaster = dto.PlayerName == username;
            var gameStatus = isMaster ? CreateGameStatusForMaster(dto) : CreateGameStatusForOpponent(dto);
            await responseStream.WriteAsync(gameStatus);
            _logger.LogDebug($"{username} -> Updated {gameStatus.User} vs {gameStatus.Challenger} /{gameStatus.UserPick}-{gameStatus.ChallengerPick}/");
            while (!context.CancellationToken.IsCancellationRequested && gameStatus.Result == Result.Pending)
            {
                await Task.Delay(_multiplayerSettings.GameStatusUpdateDelay);
                dto = await _repository.GetMatch(request.MatchId);

                if (dto == null)
                {
                    _logger.LogDebug($"{username} -> dto is null");
                    await responseStream.WriteAsync(_cancelledMatch);
                    return;
                }

                var matchExpired = DateTime.UtcNow.AddSeconds(-_multiplayerSettings.GameStatusMaxWait) > dto.WhenUtc;
                if (isMaster && matchExpired)
                {
                    _logger.LogDebug($"{username} -> match expired");
                    await _repository.DeleteMatch(request.MatchId);
                    await responseStream.WriteAsync(_cancelledMatch);
                    return;
                }

                gameStatus = isMaster ? CreateGameStatusForMaster(dto) : CreateGameStatusForOpponent(dto);
                _logger.LogDebug($"{username} -> Updated {gameStatus.User} vs {gameStatus.Challenger} /{gameStatus.UserPick}-{gameStatus.ChallengerPick}/");
                await responseStream.WriteAsync(gameStatus);
            }
        }

        public override async Task<Empty> Pick(PickRequest request, ServerCallContext context)
        {
            var username = GetUsername(request.Username, request.TwitterLogged);
            var dto = await _repository.SaveMatchPick(request.MatchId, username, request.Pick);
            if (!string.IsNullOrWhiteSpace(dto.ChallengerMove?.Text) && !string.IsNullOrWhiteSpace(dto.PlayerMove?.Text))
            {
                var result = _gameService.Check(dto.PlayerMove.Value, dto.ChallengerMove.Value);
                await _playFabService.UpdateStats(dto.PlayerName, result == Result.Player);
                await _playFabService.UpdateStats(dto.Challenger.Name, result == Result.Challenger);
                await _repository.SaveMatchResult(request.MatchId, result);
            }

            return new Empty();
        }

        public override async Task Rematch(RematchRequest request, IServerStreamWriter<RematchResponse> responseStream, ServerCallContext context)
        {
            var username = GetUsername(request.Username, request.TwitterLogged);
            var dto = await _repository.GetMatch(request.MatchId);
            if (dto.Result.Value == (int)Result.Pending)
            {
                await _repository.SaveMatchChallenger(request.MatchId, username);
                await responseStream.WriteAsync(new RematchResponse { HasStarted = true });
                return;
            }

            await _repository.CreateMatch(request.MatchId, username, UnknownUser);
            await responseStream.WriteAsync(new RematchResponse());
            dto = await _repository.GetMatch(request.MatchId);
            while (!context.CancellationToken.IsCancellationRequested && dto.Challenger.Name == UnknownUser)
            {
                await Task.Delay(_multiplayerSettings.GameStatusUpdateDelay);
                dto = await _repository.GetMatch(request.MatchId);

                if (dto == null)
                {
                    _logger.LogDebug($"{username} -> dto is null");
                    return;
                }

                var matchExpired = DateTime.UtcNow.AddSeconds(-_multiplayerSettings.GameStatusMaxWait) > dto.WhenUtc;
                if (matchExpired)
                {
                    _logger.LogDebug($"{username} -> rematch expired");
                    await _repository.DeleteMatch(request.MatchId);
                    return;
                }

                await responseStream.WriteAsync(new RematchResponse());
            }

            await responseStream.WriteAsync(new RematchResponse { HasStarted = true });
        }

        public override async Task<LeaderboardResponse> Leaderboard(Empty request, ServerCallContext context)
        {
            var leaderboard = await _playFabService.GetLeaderboard();
            var result = new LeaderboardResponse();
            foreach (var player in leaderboard.Players)
            {
                result.Players.Add(new LeaderboardEntryResponse()
                {
                    Username = player.Username,
                    TwitterLogged = player.IsTwitterUser,
                    Score = player.Score
                });
            }

            return result;
        }

        private static string GetUsername(string username, bool twitterLogged) 
            => twitterLogged ? username.ToUpperInvariant() : $"${username.ToUpperInvariant()}";

        private static string GetUserDisplay(string username) => 
            string.IsNullOrWhiteSpace(username) ? "-" :
            (username.StartsWith('$') ? username.Substring(1) : username);

        private static PairingStatusResponse CreateMatchStatusResponse(string status, string token, string matchId = null)
            => new PairingStatusResponse()
            {
                Status = status ?? string.Empty,
                MatchId = matchId ?? string.Empty,
                Token = token ?? string.Empty
            };

        private static GameStatusResponse CreateGameStatusForMaster(MatchDto match)
        {
            return new GameStatusResponse
            {
                User = GetUserDisplay(match.PlayerName),
                UserPick = match.PlayerMove.Value,
                Challenger = GetUserDisplay(match.Challenger?.Name),
                ChallengerPick = match.ChallengerMove.Value,
                Result = (Result)match.Result.Value,
                IsMaster = true,
                IsCancelled = false,
                IsFinished = match.Result.Value != (int)Result.Pending
            };
        }

        private static GameStatusResponse CreateGameStatusForOpponent(MatchDto match)
        {
            var result = match.Result.Value switch
            {
                1 => Result.Challenger,
                2 => Result.Player,
                _ => (Result)match.Result.Value
            };

            return new GameStatusResponse
            {
                User = GetUserDisplay(match.Challenger?.Name),
                UserPick = match.ChallengerMove.Value,
                Challenger = GetUserDisplay(match.PlayerName),
                ChallengerPick = match.PlayerMove.Value,
                Result = result,
                IsMaster = false,
                IsCancelled = false,
                IsFinished = result != Result.Pending
            };
        }
    }
}
