using PlayFab.Internal;

namespace RPSLS.Game.Multiplayer.Builders
{
    public class PlayFabRequestCommonBuilder<T, U> : BaseRequestBuilder<U>
        where T : PlayFabRequestCommonBuilder<T, U>
        where U : PlayFabRequestCommon, new()
    {
        public T WithTitleContext(string title, string token)
        {
            _product.AuthenticationContext = new PlayFab.PlayFabAuthenticationContext()
            {
                EntityType = "title",
                EntityId = title,
                EntityToken = token
            };

            return this as T;
        }

        public T WithUserContext(string userTitleId, string token)
        {
            _product.AuthenticationContext = new PlayFab.PlayFabAuthenticationContext()
            {
                EntityType = "title_player_account",
                EntityId = userTitleId,
                EntityToken = token
            };

            return this as T;
        }
    }
}
