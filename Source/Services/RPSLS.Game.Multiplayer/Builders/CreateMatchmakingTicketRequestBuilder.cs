using PlayFab.MultiplayerModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class CreateMatchmakingTicketRequestBuilder : PlayFabRequestCommonBuilder<CreateMatchmakingTicketRequestBuilder, CreateMatchmakingTicketRequest>
    {
        public CreateMatchmakingTicketRequestBuilder WithCreatorEntity(string id, string type, string token, string displayName)
        {
            _product.Creator = new MatchmakingPlayer()
            {
                Entity = new EntityKey()
                {
                    Id = id,
                    Type = type
                },
                Attributes = new MatchmakingPlayerAttributes()
                {
                    DataObject = new
                    {
                        Token = token,
                        DisplayName = displayName
                    }
                }
            };

            return this;
        }

        public CreateMatchmakingTicketRequestBuilder WithGiveUpOf(int timeout)
        {
            _product.GiveUpAfterSeconds = timeout;
            return this;
        }

        public CreateMatchmakingTicketRequestBuilder WithQueue(string queueName)
        {
            _product.QueueName = queueName;
            return this;
        }
    }
}
