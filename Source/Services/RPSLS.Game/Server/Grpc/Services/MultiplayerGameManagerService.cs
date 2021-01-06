using GameBff.Proto;
using Grpc.Core;
using System.Threading.Tasks;

namespace RPSLS.Game.Server.Grpc.Services
{
    public class MultiplayerGameManagerService : MultiplayerGameManager.MultiplayerGameManagerBase
    {
        private readonly GameApi.Proto.MultiplayerGameManager.MultiplayerGameManagerClient _multiplayerGameManagerClient;

        public MultiplayerGameManagerService(GameApi.Proto.MultiplayerGameManager.MultiplayerGameManagerClient multiplayerGameManagerClient)
        {
            _multiplayerGameManagerClient = multiplayerGameManagerClient;
        }

        public override async Task CreatePairing(CreatePairingRequest request, IServerStreamWriter<PairingStatusResponse> responseStream, ServerCallContext context)
        {
            var apiRequest = new GameApi.Proto.CreatePairingRequest() { Username = request.Username, TwitterLogged = request.TwitterLogged };
            using var stream = _multiplayerGameManagerClient.CreatePairing(apiRequest);

            GameApi.Proto.PairingStatusResponse response = null;
            while (await stream.ResponseStream.MoveNext(context.CancellationToken))
            {
                response = stream.ResponseStream.Current;

                await responseStream.WriteAsync(new PairingStatusResponse
                {
                    MatchId = response.MatchId,
                    Status = response.Status,
                    Token = response.Token
                });
            }
        }

        public override async Task JoinPairing(JoinPairingRequest request, IServerStreamWriter<PairingStatusResponse> responseStream, ServerCallContext context)
        {
            var apiRequest = new GameApi.Proto.JoinPairingRequest() { Username = request.Username, Token = request.Token, TwitterLogged = request.TwitterLogged };
            using var stream = _multiplayerGameManagerClient.JoinPairing(apiRequest);
            GameApi.Proto.PairingStatusResponse response = null;

            while (await stream.ResponseStream.MoveNext(context.CancellationToken))
            {
                response = stream.ResponseStream.Current;

                await responseStream.WriteAsync(new PairingStatusResponse
                {
                    MatchId = response.MatchId,
                    Status = response.Status,
                    Token = response.Token
                });
            }
        }

        public override async Task GameStatus(GameStatusRequest request, IServerStreamWriter<GameStatusResponse> responseStream, ServerCallContext context)
        {
            var apiRequest = new GameApi.Proto.GameStatusRequest()
            {
                MatchId = request.MatchId,
                Username = request.Username,
                TwitterLogged = request.TwitterLogged
            };

            using var stream = _multiplayerGameManagerClient.GameStatus(apiRequest);

            while (await stream.ResponseStream.MoveNext(context.CancellationToken))
            {
                var response = stream.ResponseStream.Current;
                await responseStream.WriteAsync(new GameStatusResponse
                {
                    Challenger = response.Challenger,
                    ChallengerPick = response.ChallengerPick,
                    User = response.User,
                    UserPick = response.UserPick,
                    Result = (Result)(int)response.Result,
                    IsFinished = response.IsFinished,
                    IsCancelled = response.IsCancelled,
                    IsMaster = response.IsMaster
                });
            }
        }

        public override async Task<Empty> Pick(PickRequest request, ServerCallContext context)
        {
            var apiRequest = new GameApi.Proto.PickRequest()
            {
                MatchId = request.MatchId,
                Username = request.Username,
                TwitterLogged = request.TwitterLogged,
                Pick = request.Pick
            };

            await _multiplayerGameManagerClient.PickAsync(apiRequest);

            return new Empty();
        }

        public override async Task Rematch(RematchRequest request, IServerStreamWriter<RematchResponse> responseStream, ServerCallContext context)
        {
            var apiRequest = new GameApi.Proto.RematchRequest()
            {
                MatchId = request.MatchId,
                Username = request.Username,
                TwitterLogged = request.TwitterLogged
            };

            using var stream = _multiplayerGameManagerClient.Rematch(apiRequest);
            while (await stream.ResponseStream.MoveNext(context.CancellationToken))
            {
                var response = stream.ResponseStream.Current;

                await responseStream.WriteAsync(new RematchResponse
                {
                    HasStarted = response.HasStarted,
                    IsCancelled = response.IsCancelled
                });
            }
        }

        public override async Task<LeaderboardResponse> Leaderboard(Empty request, ServerCallContext context)
        {
            var leaderboard = await _multiplayerGameManagerClient.LeaderboardAsync(new GameApi.Proto.Empty());

            var result = new LeaderboardResponse();
            foreach (var player in leaderboard.Players)
            {
                result.Players.Add(new LeaderboardEntryResponse()
                {
                    Username = player.Username,
                    TwitterLogged = player.TwitterLogged,
                    Score = player.Score
                });
            }

            return result;
        }
    }
}
