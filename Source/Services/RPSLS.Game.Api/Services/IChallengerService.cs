using GameApi.Proto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Services
{
    public interface IChallengerService
    {

        public IEnumerable<IChallenger> Challengers { get; }

        IChallenger SelectChallenger(GameRequest request);
    }
}
