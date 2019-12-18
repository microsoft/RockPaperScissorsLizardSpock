using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RPSLS.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private const string redirectUri = "/challenger";

        [HttpGet("{token}")]
        public IActionResult JoinGame(string token)
        {
            var redirect = $"{redirectUri}?token={token}";
            return Challenge(new AuthenticationProperties { RedirectUri = redirect }, "Twitter");
        }
    }
}