namespace RPSLS.Game.Shared.Config
{
    public class ClientSettings
    {
        public GoogleAnalyticsSettings GoogleAnalyticsSettings { get; set; }
        public MultiplayerSettings MultiplayerSettings { get; set; }
        public RecognitionSettings RecognitionSettings { get; set; }
        public TwitterSettings TwitterSettings { get; set; }
        public GameSettingsDto GameSettingsDto { get; set; }
        public GameManagerSettings GameManagerSettings { get; set; }
    }
}
