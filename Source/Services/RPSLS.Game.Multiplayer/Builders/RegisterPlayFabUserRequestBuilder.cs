using PlayFab.ClientModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class UpdateUserTitleDisplayNameRequestBuilder : PlayFabRequestCommonBuilder<UpdateUserTitleDisplayNameRequestBuilder, UpdateUserTitleDisplayNameRequest>
    {
        public UpdateUserTitleDisplayNameRequestBuilder WithName(string displayName)
        {
            _product.DisplayName = displayName;
            return this;
        }
    }
}
