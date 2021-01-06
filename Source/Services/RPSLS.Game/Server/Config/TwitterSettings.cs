using Microsoft.AspNetCore.Authentication.Twitter;

namespace RPSLS.Game.Server.Config
{
    public class TwitterSettings
    {
        public string AuthenticationScheme = TwitterDefaults.AuthenticationScheme;
        public bool IsLoginEnabled { get; set; }
    }
}
