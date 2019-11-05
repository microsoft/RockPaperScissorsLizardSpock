using GameApi.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Services
{
    public interface IChallenger
    {
        ChallengerDto Info { get; }
        Task<PickDto> Pick(IDictionary<string, string> context, bool isTwitterLogged, string userName);
    }
}
