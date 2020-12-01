namespace RPSLS.Game.Shared.Config
{
    public class TwitterSettings
    {
        public string AuthenticationScheme = "Twitter";
        public bool IsLoginEnabled { get; set; }

        public TwitterSettings() { }

        public TwitterSettings(string consumerKey)
        {
            IsLoginEnabled = !string.IsNullOrEmpty(consumerKey);
        }
    }
}
