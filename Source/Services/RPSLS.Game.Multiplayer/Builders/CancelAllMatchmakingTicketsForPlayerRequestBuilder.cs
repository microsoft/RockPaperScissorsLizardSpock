using PlayFab.MultiplayerModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class CancelAllMatchmakingTicketsForPlayerRequestBuilder : PlayFabRequestCommonBuilder<
        CancelAllMatchmakingTicketsForPlayerRequestBuilder,
        CancelAllMatchmakingTicketsForPlayerRequest>
    {
        public CancelAllMatchmakingTicketsForPlayerRequestBuilder WithEntity(string id, string type)
        {
            _product.Entity = new EntityKey()
            {
                Type = type,
                Id = id
            };
            return this;
        }
        
        public CancelAllMatchmakingTicketsForPlayerRequestBuilder WithQueue(string queueName)
        {
            _product.QueueName = queueName;
            return this;
        }
    }
}
