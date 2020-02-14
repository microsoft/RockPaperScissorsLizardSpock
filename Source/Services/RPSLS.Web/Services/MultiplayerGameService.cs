using RPSLS.Web.Clients;
using RPSLS.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Web.Services
{
    public class MultiplayerGameService : GameService, IMultiplayerGameService
    {
        private readonly IMultiplayerGameManagerClient _gameManager;

        public MultiplayerGameService(IMultiplayerGameManagerClient gameManager)
        {
            _gameManager = gameManager;
        }

        public string MatchId { get; set; }

        public async Task FetchMatchId(Action<string, string, string> matchIdCallback)
        {
            MatchId = await _gameManager.CreatePairing(Username, IsTwitterUser, matchIdCallback);
        }

        public async Task FetchMatchId(string token)
        {
            MatchId = await _gameManager.JoinPairing(Username, IsTwitterUser, token);
        }

        public async Task AddGameListener(Action<ResultDto> gameListener)
        {
            await _gameManager.GameStatus(MatchId, Username, IsTwitterUser, gameListener);
        }

        public async Task UserPick(int pick)
        {
            Pick = pick;
            await _gameManager.Pick(MatchId, Username, IsTwitterUser, pick);
        }

        public async Task<bool> Rematch() => await _gameManager.Rematch(MatchId, Username, IsTwitterUser);
        public async Task<LeaderboardDto> GetLeaderboard()
        {
            var leaderboard = await _gameManager.GetLeaderboard();
            return new LeaderboardDto()
            {
                Players = leaderboard.Players.Select((p, i) => new LeaderboardEntryDto
                {
                    Position = i + 1,
                    Username = p.Username,
                    Score = p.Score
                })
            };
        }
    }
}
