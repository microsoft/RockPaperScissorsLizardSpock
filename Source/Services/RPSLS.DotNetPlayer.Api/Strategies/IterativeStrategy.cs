using RPSLS.DotNetPlayer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPSLS.DotNetPlayer.API.Strategies
{
    public class IterativeStrategy : IStrategy
    {
        private static int _nextPick = 0;
        private readonly List<RPSLSEnum> Picks = new List<RPSLSEnum>();

        public IterativeStrategy()
        {
            Picks = Enum.GetValues(typeof(RPSLSEnum)).Cast<RPSLSEnum>().ToList();
        }

        public Choice GetChoice()
        {
            var choice = Picks[_nextPick++];
            _nextPick %= Picks.Count;
            return new Choice(choice);
        }
    }
}
