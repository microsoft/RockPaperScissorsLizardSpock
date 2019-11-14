using RPSLS.DotNetPlayer.Api.Models;
using System;

namespace RPSLS.DotNetPlayer.API.Strategies
{
    public class RandomStrategy : IStrategy
    {
        public Choice GetChoice()
        {
            Random random = new Random();
            var values = Enum.GetValues(typeof(RPSLSEnum));
            return new Choice(random.Next(values.Length));
        }
    }
}
