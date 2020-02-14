using PlayFab.MultiplayerModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class ListMatchmakingTicketsForPlayerRequestBuilder : PlayFabRequestCommonBuilder<ListMatchmakingTicketsForPlayerRequestBuilder, ListMatchmakingTicketsForPlayerRequest>
    {
        public ListMatchmakingTicketsForPlayerRequestBuilder WithQueue(string queueName)
        {
            _product.QueueName = queueName;
            return this;
        }

        public ListMatchmakingTicketsForPlayerRequestBuilder WithEntity(string id, string type)
        {
            _product.Entity = new EntityKey()
            {
                Type = type,
                Id = id
            };
            return this;
        }
    }
}
