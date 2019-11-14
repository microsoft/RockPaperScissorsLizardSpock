using GameApi.Proto;
using System;

namespace RPSLS.Game.Api.Services
{
    public class GameService : IGameService
    {
        public Result Check(int player, int challenger)
        {
            if (player == challenger)
                return Result.Tie;

            return player switch
            {
                0 => challenger == 2 || challenger == 3 ? Result.Player : Result.Challenger,
                1 => challenger == 0 || challenger == 4 ? Result.Player : Result.Challenger,
                2 => challenger == 1 || challenger == 3 ? Result.Player : Result.Challenger,
                3 => challenger == 1 || challenger == 4 ? Result.Player : Result.Challenger,
                4 => challenger == 0 || challenger == 2 ? Result.Player : Result.Challenger,

                _ => throw new InvalidOperationException($"Invalid player pick {player}"),
            };
        }
    }
}
