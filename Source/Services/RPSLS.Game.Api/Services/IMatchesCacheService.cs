using RPSLS.Game.Api.Data.Models;

namespace RPSLS.Game.Api.Services
{
    public interface IMatchesCacheService
    {
        void CreateMatch(MatchDto matchDto);
        MatchDto GetMatch(string matchId);
        MatchDto UpdateMatch(MatchDto updatedMatch);
        void DeleteMatch(string matchId);
    }
}
