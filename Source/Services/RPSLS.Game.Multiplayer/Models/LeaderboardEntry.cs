namespace RPSLS.Game.Multiplayer.Models
{
    public class LeaderboardEntry
    {
        public int Position { get; set; }
        public string Username { get; set; }
        public bool IsTwitterUser { get; set; }
        public int Score { get; set; }
    }
}
