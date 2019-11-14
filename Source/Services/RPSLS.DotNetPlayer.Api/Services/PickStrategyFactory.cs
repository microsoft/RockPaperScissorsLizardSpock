using RPSLS.DotNetPlayer.Api.Models;
using RPSLS.DotNetPlayer.API.Strategies;

namespace RPSLS.DotNetPlayer.API.Services
{
    public class PickStrategyFactory : IPickStrategyFactory
    {
        private const string _ROCK = "rock";
        private const string _PAPER = "paper";
        private const string _SCISSORS = "scissors";
        private const string _LIZARD = "lizard";
        private const string _SPOCK = "spock";
        private const string _RANDOM = "random";
        private const string _ITERATIVE = "iterative";

        private string _defaultStrategy = _RANDOM;

        public IStrategy GetStrategy()
        {
            switch(_defaultStrategy)
            {
                case _ROCK:
                    return new FixedStrategy(RPSLSEnum.Rock);
                case _PAPER:
                    return new FixedStrategy(RPSLSEnum.Paper);
                case _SCISSORS:
                    return new FixedStrategy(RPSLSEnum.Scissors);
                case _LIZARD:
                    return new FixedStrategy(RPSLSEnum.Lizard);
                case _SPOCK:
                    return new FixedStrategy(RPSLSEnum.Spock);
                case _ITERATIVE:
                    return new IterativeStrategy();
                case _RANDOM:
                default:
                    return new RandomStrategy();
            }
        }

        public void SetDefaultStrategy(string newDefaultStrategy)
        {
            if(!string.IsNullOrEmpty(newDefaultStrategy))
            {
                _defaultStrategy = newDefaultStrategy.ToLower();
            }
        }
    }
}
