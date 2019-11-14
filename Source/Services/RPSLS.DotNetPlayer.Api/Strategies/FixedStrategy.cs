using RPSLS.DotNetPlayer.Api.Models;

namespace RPSLS.DotNetPlayer.API.Strategies
{
    public class FixedStrategy : IStrategy
    {
        private readonly RPSLSEnum _pick;

        public FixedStrategy(RPSLSEnum pick)
        {
            _pick = pick;
        }

        public Choice GetChoice()
        {
            return new Choice(_pick);
        }
    }
}
