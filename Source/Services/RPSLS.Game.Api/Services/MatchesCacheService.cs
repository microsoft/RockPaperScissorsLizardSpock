using RPSLS.Game.Api.Data.Models;
using System.Collections.Generic;

namespace RPSLS.Game.Api.Services
{
    public class MatchesCacheService : IMatchesCacheService
    {
        private readonly object _lockObj = new object();
        private readonly Dictionary<string, MatchDto> _cache = new Dictionary<string, MatchDto>();

        public void CreateMatch(MatchDto matchDto)
        {
            lock (_lockObj)
            {
                if (_cache.ContainsKey(matchDto.PlayFabMatchId))
                    _cache[matchDto.PlayFabMatchId] = matchDto;
                else
                    _cache.Add(matchDto.PlayFabMatchId, matchDto);
            }
        }

        public MatchDto GetMatch(string matchId) => _cache.ContainsKey(matchId) ? _cache[matchId] : null;

        public MatchDto UpdateMatch(MatchDto updatedMatch)
        {
            lock (_lockObj)
            {
                var matchId = updatedMatch.PlayFabMatchId;
                _cache[matchId] = updatedMatch;
                return updatedMatch;
            }
        }

        public void DeleteMatch(string matchId)
        {
            lock (_lockObj)
            {
                if (_cache.ContainsKey(matchId))
                    _cache.Remove(matchId);
            }
        }
    }
}
