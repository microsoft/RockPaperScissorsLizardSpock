using GameBff.Proto;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace RPSLS.Game.Server.GrpcServices
{
    public class BotGameManagerService : GameBff.Proto.BotGameManager.BotGameManagerBase
    {

        private readonly ILogger<BotGameManagerService> _logger;

        public BotGameManagerService(ILogger<BotGameManagerService> logger)
        {

            _logger = logger;
        }

        public override async Task<GameBff.Proto.ChallengersList> GetChallengers(GameBff.Proto.Empty request, ServerCallContext context)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5102");
            var client = new GameApi.Proto.BotGameManager.BotGameManagerClient(channel);
            var challengersListApi = await client.GetChallengersAsync(new GameApi.Proto.Empty(), GetRequestMetadata());
            var result = new ChallengersList();
            foreach (var challenger in challengersListApi.Challengers)
            {
                result.Challengers.Add(new ChallengerInfo { Name = challenger.Name, DisplayName = challenger.DisplayName });
            }

            result.Count = result.Challengers.Count;

            return result;
        }


        protected Grpc.Core.Metadata GetRequestMetadata()
        {
            var metadata = new Grpc.Core.Metadata();
            return metadata;
        }
    }
}
