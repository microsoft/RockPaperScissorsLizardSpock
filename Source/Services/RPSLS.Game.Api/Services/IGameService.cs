using GameApi.Proto;

namespace RPSLS.Game.Api.Services
{
    public interface IGameService
    {
        Result Check(int player, int challenger);
    }
}
