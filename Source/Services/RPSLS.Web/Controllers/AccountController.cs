using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPSLS.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private const string REDIRECT_URI = "/";

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet("login/twitter")]
        public IActionResult ExternalLogin([FromQuery] string redirectUrl)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl ?? REDIRECT_URI };
            _logger.LogInformation($"Twitter login redirected to {properties.RedirectUri}");
            return base.Challenge(properties, TwitterDefaults.AuthenticationScheme);
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string redirectUrl)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(principal);
            _logger.LogInformation($"Cookies login redirected to {redirectUrl ?? REDIRECT_URI}");
            return Redirect(redirectUrl ?? REDIRECT_URI);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation($"Logout Completed");
            return Redirect(REDIRECT_URI);
        }
    }
}