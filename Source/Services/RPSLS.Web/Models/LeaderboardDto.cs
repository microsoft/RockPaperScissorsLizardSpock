using System.Collections.Generic;

namespace RPSLS.Web.Models
{
    public class LeaderboardDto
    {
        public IEnumerable<LeaderboardEntryDto> Players { get; set; }
    }
}
