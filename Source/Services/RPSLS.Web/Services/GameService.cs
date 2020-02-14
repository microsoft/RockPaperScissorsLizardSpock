using RPSLS.Web.Models;

namespace RPSLS.Web.Services
{
    public abstract class GameService
    {
        public string Username { get; set;}
        public bool IsTwitterUser { get; set; }

        public int Pick { get; set; }
        public ChallengerDto Challenger { get; set; }
        public ResultDto GameResult { get; set; }
    }
}
