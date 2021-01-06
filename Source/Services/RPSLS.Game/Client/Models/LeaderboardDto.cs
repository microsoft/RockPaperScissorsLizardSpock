using System.Collections.Generic;

namespace RPSLS.Game.Client.Models
{
    public class LeaderboardDto
    {
        public IEnumerable<LeaderboardEntryDto> Players { get; set; }
    }
}
