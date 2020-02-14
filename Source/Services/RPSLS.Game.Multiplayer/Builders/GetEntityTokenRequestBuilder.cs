using PlayFab.AuthenticationModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetEntityTokenRequestBuilder : PlayFabRequestCommonBuilder<GetEntityTokenRequestBuilder, GetEntityTokenRequest>
    {
        public GetEntityTokenRequestBuilder WithUserToken(string userTitleId)
        {
            _product.Entity = new EntityKey()
            {
                Id = userTitleId,
                Type = "title_player_account"
            };

            return this;
        }

        public GetEntityTokenRequestBuilder WithTitleToken(string titleId)
        {
            _product.Entity = new EntityKey()
            {
                Id = titleId,
                Type = "title"
            };

            return this;
        }
    }
}
