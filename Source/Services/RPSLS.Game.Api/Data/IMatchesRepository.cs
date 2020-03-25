using RPSLS.Game.Api.Data.Models;
using RPSLS.Game.Api.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Data
{
    public interface IMatchesRepository
    {
        Task CreateMatch(string matchId, string username, string challenger);
        Task<MatchDto> GetMatch(string matchId);
        Task<MatchDto> SaveMatchPick(string matchId, string username, int pick);
        Task<MatchDto> SaveMatchResult(string matchId, GameApi.Proto.Result result);
        Task<MatchDto> SaveMatchChallenger(string matchId, string username);
        Task DeleteMatch(string matchId);

        Task SaveMatch(PickDto pick, string username, int userPick, GameApi.Proto.Result result);
        Task<IEnumerable<MatchDto>> GetLastGamesOfPlayer(string player, int limit);
    }
}
