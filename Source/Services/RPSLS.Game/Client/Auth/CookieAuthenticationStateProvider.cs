using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using RPSLS.Game.Shared.Models;

namespace RPSLS.Game.Client.Auth
{
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        public CookieAuthenticationStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var httpResponseMessage = await _httpClient.GetAsync("api/account/userinfo");
            var userInfo = await httpResponseMessage.Content.ReadFromJsonAsync<UserInfo>();

            ClaimsIdentity identity = default;

            if (!string.IsNullOrEmpty(userInfo?.Username))
            {
                identity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userInfo.Username) }, userInfo.AuthenticationType);
            }

            var claimsPrincipal = new ClaimsPrincipal(identity ?? new ClaimsIdentity());
            var authenticationState = new AuthenticationState(claimsPrincipal);

            return authenticationState;
        }
    }
}
