using System.Collections.Generic;

namespace RPSLS.Game.Multiplayer.Models
{
    public class Leaderboard
    {
        public IEnumerable<LeaderboardEntry> Players { get; set; }
    }
}
