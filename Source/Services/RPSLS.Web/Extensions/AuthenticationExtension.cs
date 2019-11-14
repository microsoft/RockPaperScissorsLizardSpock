using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationExtension
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var auth = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            auth.AddCookie();
            var consumerKey = configuration["Authentication:Twitter:ConsumerKey"];
            var consumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];
            if (!string.IsNullOrWhiteSpace(consumerKey) && !string.IsNullOrWhiteSpace(consumerSecret))
            {
                auth.AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = consumerKey;
                    twitterOptions.ConsumerSecret = consumerSecret;
                });
            }
        }
    }
}
