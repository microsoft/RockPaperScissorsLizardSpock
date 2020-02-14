using PlayFab.ClientModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetAccountInfoRequestBuilder : PlayFabRequestCommonBuilder<GetAccountInfoRequestBuilder, GetAccountInfoRequest>
    {
        public GetAccountInfoRequestBuilder WithPlayFabId(string playFabId)
        {
            _product.PlayFabId = playFabId;
            return this;
        }

        public GetAccountInfoRequestBuilder WithDisplayName(string displayName)
        {
            _product.TitleDisplayName = displayName;
            return this;
        }
    }
}
