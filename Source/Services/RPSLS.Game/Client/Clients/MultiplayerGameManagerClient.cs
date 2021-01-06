using GameBff.Proto;
using RPSLS.Game.Client.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RPSLS.Game.Client.Clients
{
    public class MultiplayerGameManagerClient : IMultiplayerGameManagerClient
    {
        private readonly MultiplayerGameManager.MultiplayerGameManagerClient _multiplayerGameManagerClient;

        public MultiplayerGameManagerClient(MultiplayerGameManager.MultiplayerGameManagerClient multiplayerGameManagerClient)
        {
            _multiplayerGameManagerClient = multiplayerGameManagerClient;
        }

        public async Task<string> CreatePairing(string username, bool isTwitterUser, Action<string, string, string> matchIdCallback)
        {
            var request = new CreatePairingRequest() { Username = username, TwitterLogged = isTwitterUser };
            using var stream = _multiplayerGameManagerClient.CreatePairing(request);
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
            using var stream = _multiplayerGameManagerClient.JoinPairing(request);
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
            await _multiplayerGameManagerClient.PickAsync(request);
        }

        public async Task<ResultDto> GameStatus(string matchId, string username, bool isTwitterUser, Action<ResultDto> gameListener)
        {
            var request = new GameStatusRequest()
            {
                MatchId = matchId,
                Username = username,
                TwitterLogged = isTwitterUser
            };

            using var stream = _multiplayerGameManagerClient.GameStatus(request);
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

            using var stream = _multiplayerGameManagerClient.Rematch(request);
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var response = stream.ResponseStream.Current;
                if (response.HasStarted) return true;
            }

            return false;
        }

        public async Task<LeaderboardDto> GetLeaderboard()
        {
            var leaderboard = await _multiplayerGameManagerClient.LeaderboardAsync(new Empty());
            return new LeaderboardDto()
            {
                Players = leaderboard.Players.Select(
                    p => new LeaderboardEntryDto { Username = p.Username, Score = p.Score })
            };
        }
    }
}
