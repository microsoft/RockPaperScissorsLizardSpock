using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Web.Helpers
{
    public class BattleHelper
    {
        public string GetAnimatedClass(bool isShaking)
        {
            if (isShaking)
            {
                return "animated";
            }
            else
            {
                return "animated-end";
            }
        }

        public string GetHandIcon(string userHandToShow)
        {
            if (userHandToShow == "lizard")
            {
                return $"{userHandToShow}_animated";
            }
            return userHandToShow;
        }

        public string GetHandIcon(int challengerPick)
        {
            var hand = challengerPick.ToString();
            if (hand == "3")
            {
                return $"{hand}_animated";
            }
            return hand;
        }

    }
}
