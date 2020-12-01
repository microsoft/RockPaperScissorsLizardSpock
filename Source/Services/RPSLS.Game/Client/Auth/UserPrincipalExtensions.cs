using System.Linq;
using System.Security.Claims;

namespace RPSLS.Game.Client.Auth
{
    public static class UserPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal User)
        {
            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }
    }
}
