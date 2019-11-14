using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPSLS.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private const string redirectUri = "/challenger";

        [HttpGet("login/twitter")]
        public IActionResult ExternalLogin() =>
            Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, "Twitter");

        [HttpGet("login")]
        public async Task<IActionResult> Login(string username)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(principal);
            return Redirect(redirectUri);
        }
    }
}