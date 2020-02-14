using RPSLS.Web.Models;

namespace RPSLS.Web.Services
{
    public interface IGameService
    {
        string Username { get; set; }
        bool IsTwitterUser { get; set; }
        int Pick { get; set; }
        ChallengerDto Challenger { get; set; }
        ResultDto GameResult { get; set; }
    }
}
