using PlayFab.MultiplayerModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetMatchRequestBuilder : PlayFabRequestCommonBuilder<GetMatchRequestBuilder, GetMatchRequest>
    {
        public GetMatchRequestBuilder WithQueue(string queueName)
        {
            _product.QueueName = queueName;
            return this;
        }

        public GetMatchRequestBuilder WithId(string id)
        {
            _product.MatchId = id;
            return this;
        }

        public GetMatchRequestBuilder WithMemberAttributes()
        {
            _product.ReturnMemberAttributes = true;
            return this;
        }
    }
}
