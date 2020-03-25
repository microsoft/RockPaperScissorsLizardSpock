using GameApi.Proto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Services
{
    public interface IChallenger
    {
        ChallengerInfo Info { get; }
        Task<PickDto> Pick(IDictionary<string, string> context, bool isTwitterLogged, string userName);
    }
}
