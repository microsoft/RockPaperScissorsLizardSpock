using GameApi.Proto;
using RPSLS.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Web.Clients
{
    public interface IGameManagerClient
    {
        Task<ResultDto> Play(string challenger, string username, int pick, bool twitterLogged);

        Task<IEnumerable<ChallengerDto>> Challengers();
    }
}
