using RPSLS.Game.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Game.Client.Clients
{
    public interface IBotGameManagerClient
    {
        Task<ResultDto> Play(string challenger, string username, int pick, bool twitterLogged);

        Task<IEnumerable<ChallengerDto>> Challengers();
    }
}
