using RPSLS.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Web.Services
{
    public interface IBotGameService : IGameService
    {
        Task Play();
        Task<IEnumerable<ChallengerDto>> Challengers();
    }
}
