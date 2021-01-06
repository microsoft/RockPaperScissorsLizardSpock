using RPSLS.Game.Client.Models;

namespace RPSLS.Game.Client.Services
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
