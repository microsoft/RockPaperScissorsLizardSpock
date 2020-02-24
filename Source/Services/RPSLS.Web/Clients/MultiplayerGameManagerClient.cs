using GameApi.Proto;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RPSLS.Web.Config;
using RPSLS.Web.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RPSLS.Web.Clients
{
    public class MultiplayerGameManagerClient : BaseClient, IMultiplayerGameManagerClient
    {
        private readonly string _serverUrl;

        public MultiplayerGameManagerClient(IOptions<GameManagerSettings> settings, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _serverUrl = settings.Value.Url ?? throw new ArgumentNullException("Game Manager Url is null");
        }

        public async Task<string> CreatePairing(string username, bool isTwitterUser, Action<string, string, string> matchIdCallback)
        {
            var request = new CreatePairingRequest() { Username = username, TwitterLogged = isTwitterUser };
            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new MultiplayerGameManager.MultiplayerGameManagerClient(channel);
            using var stream = client.CreatePairing(request, GetRequestMetadata());
            PairingStatusResponse response = null;
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                response = stream.ResponseStream.Current;
                matchIdCallback(response.MatchId, response.Status, response.Token);
            }

            return response.MatchId;
        }

        public async Task<string> JoinPairing(string username, bool isTwitterUser, string token)
        {
            var request = new JoinPairingRequest() { Username = username, Token = token, TwitterLogged = isTwitterUser };
            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new MultiplayerGameManager.MultiplayerGameManagerClient(channel);
            using var stream = client.JoinPairing(request, GetRequestMetadata());
            PairingStatusResponse response = null;
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                response = stream.ResponseStream.Current;
            }

            return response.MatchId;
        }

        public async Task Pick(string matchId, string username, bool isTwitterUser, int pick)
        {
            var request = new PickRequest()
            {
                MatchId = matchId,
                Username = username,
                TwitterLogged = isTwitterUser,
                Pick = pick
            };
            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new MultiplayerGameManager.MultiplayerGameManagerClient(channel);
            await client.PickAsync(request, GetRequestMetadata());
        }

        public async Task<ResultDto> GameStatus(string matchId, string username, bool isTwitterUser, Action<ResultDto> gameListener)
        {
            var request = new GameStatusRequest()
            {
                MatchId = matchId,
                Username = username,
                TwitterLogged = isTwitterUser
            };

            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new MultiplayerGameManager.MultiplayerGameManagerClient(channel);
            using var stream = client.GameStatus(request, GetRequestMetadata());
            ResultDto resultDto = null;
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var response = stream.ResponseStream.Current;
                resultDto = new ResultDto
                {
                    Challenger = response.Challenger,
                    ChallengerPick = response.ChallengerPick,
                    User = response.User,
                    UserPick = response.UserPick,
                    Result = (int)response.Result,
                    IsValid = true,
                    IsFinished = response.IsFinished,
                    IsGameInitiator = response.IsMaster
                };

                gameListener(resultDto);
            }

            return resultDto;
        }

        public async Task<bool> Rematch(string matchId, string username, bool isTwitterUser)
        {
            var request = new RematchRequest()
            {
                MatchId = matchId,
                Username = username,
                TwitterLogged = isTwitterUser
            };

            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new MultiplayerGameManager.MultiplayerGameManagerClient(channel);
            using var stream = client.Rematch(request);
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var response = stream.ResponseStream.Current;
                if (response.HasStarted) return true;
            }

            return false;
        }

        public async Task<LeaderboardDto> GetLeaderboard()
        {
            var channel = GrpcChannel.ForAddress(_serverUrl);
            var client = new MultiplayerGameManager.MultiplayerGameManagerClient(channel);
            var leaderboard = await client.LeaderboardAsync(new Empty());
            return new LeaderboardDto()
            {
                Players = leaderboard.Players.Select(
                    p => new LeaderboardEntryDto { Username = p.Username, Score = p.Score })
            };
        }
    }
}
