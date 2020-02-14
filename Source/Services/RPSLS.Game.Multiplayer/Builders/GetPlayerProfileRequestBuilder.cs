using PlayFab.ClientModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetPlayerProfileRequestBuilder : PlayFabRequestCommonBuilder<GetPlayerProfileRequestBuilder, GetPlayerProfileRequest>
    {
        public GetPlayerProfileRequestBuilder WithShowDisplayName()
        {
            _product.ProfileConstraints.ShowDisplayName = true;
            return this;
        }
    }
}
