using RPSLS.DotNetPlayer.API.Strategies;

namespace RPSLS.DotNetPlayer.API.Services
{
    public interface IPickStrategyFactory
    {
        IStrategy GetStrategy();
        void SetDefaultStrategy(string newDefaultStrategy);
    }
}
