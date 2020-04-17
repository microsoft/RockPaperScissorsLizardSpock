namespace RPSLS.Game.Multiplayer.Config
{
    public class MultiplayerSettings
    {
        public string Title { get; set; }
        public string SecretKey { get; set; }

        public LeaderboardSettings Leaderboard { get; set; } = new LeaderboardSettings();

        public TokenSettings Token { get; set; } = new TokenSettings();
        public int GameStatusUpdateDelay { get; set; } = 1000;
        public int GameStatusMaxWait { get; set; } = 60;
        public int EntityTokenExpirationMinutes { get; set; } = 60;
    }
}
