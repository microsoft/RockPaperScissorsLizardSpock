using RPSLS.Web.Models;
using System;
using System.Threading.Tasks;

namespace RPSLS.Web.Clients
{
    public interface IMultiplayerGameManagerClient
    {
        Task<string> CreatePairing(string username, bool isTwitterUser, Action<string, string, string> matchIdCallback);

        Task<string> JoinPairing(string username, bool isTwitterUser, string token);

        Task Pick(string matchId, string username, bool isTwitterUser, int pick);

        Task<ResultDto> GameStatus(string matchId, string username, bool isTwitterUser, Action<ResultDto> gameListener);

        Task<bool> Rematch(string matchId, string username, bool isTwitterUser);

        Task<LeaderboardDto> GetLeaderboard();
    }
}
