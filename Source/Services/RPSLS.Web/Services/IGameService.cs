using GameApi.Proto;
using RPSLS.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPSLS.Web.Services
{
    public interface IGameService
    {
        ChallengerDto Challenger { get; set; }
        int Pick { get; set; }
        ResultDto GameResult { get; set; }
        Task Play(string username, bool isTwitterUser);
        Task<IEnumerable<ChallengerDto>> Challengers();
    }
}
