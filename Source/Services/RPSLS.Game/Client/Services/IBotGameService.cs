using RPSLS.Game.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Game.Client.Services
{
    public interface IBotGameService : IGameService
    {
        Task Play();
        Task<IEnumerable<ChallengerDto>> Challengers();
    }
}
