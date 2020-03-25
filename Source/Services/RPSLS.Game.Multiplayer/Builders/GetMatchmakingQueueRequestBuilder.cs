using PlayFab.MultiplayerModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetMatchmakingQueueRequestBuilder : PlayFabRequestCommonBuilder<GetMatchmakingQueueRequestBuilder, GetMatchmakingQueueRequest>
    {
        public GetMatchmakingQueueRequestBuilder WithQueue(string queueName)
        {
            _product.QueueName = queueName;
            return this;
        }
    }
}
