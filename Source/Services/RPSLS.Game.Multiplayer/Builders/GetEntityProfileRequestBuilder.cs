using PlayFab.ProfilesModels;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class GetEntityProfileRequestBuilder : PlayFabRequestCommonBuilder<GetEntityProfileRequestBuilder, GetEntityProfileRequest>
    {
        public GetEntityProfileRequestBuilder WithTitleEntity(string id)
        {
            _product.Entity = new EntityKey()
            {
                Id = id,
                Type = "title_player_account"
            };
            return this;
        }
    }
}
