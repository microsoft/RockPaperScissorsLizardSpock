using PlayFab.AdminModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class LookupUserAccountInfoRequestBuilder : PlayFabRequestCommonBuilder<LookupUserAccountInfoRequestBuilder, LookupUserAccountInfoRequest>
    {
        public LookupUserAccountInfoRequestBuilder WithTitleDisplay(string displayName)
        {
            _product.TitleDisplayName = displayName;
            return this;
        }
    }
}
