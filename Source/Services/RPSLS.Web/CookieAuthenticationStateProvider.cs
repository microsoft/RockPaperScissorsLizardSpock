using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPSLS.Web
{
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            var identity = new ClaimsIdentity();
            if (!string.IsNullOrWhiteSpace(username))
            {
                var authType = _httpContextAccessor.HttpContext.User.Identity.AuthenticationType;
                identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username), }, authType);
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
