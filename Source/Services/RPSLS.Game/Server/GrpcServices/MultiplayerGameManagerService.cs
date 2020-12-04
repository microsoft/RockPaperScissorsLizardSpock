using GameBff.Proto;
using Grpc.Core;
using System.Threading;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.GrpcServices
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
            using var stream = _multiplayerGameManagerClient.CreatePairing(apiRequest, GetRequestMetadata());

            GameApi.Proto.PairingStatusResponse response = null;
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
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
           
        }

        public override async Task GameStatus(GameStatusRequest request, IServerStreamWriter<GameStatusResponse> responseStream, ServerCallContext context)
        {
            
        }

        public override async Task<Empty> Pick(PickRequest request, ServerCallContext context)
        {
            return new Empty();
        }

        public override async Task Rematch(RematchRequest request, IServerStreamWriter<RematchResponse> responseStream, ServerCallContext context)
        {
            
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

        protected Grpc.Core.Metadata GetRequestMetadata()
        {
            var metadata = new Grpc.Core.Metadata();
            return metadata;
        }
    }
}
