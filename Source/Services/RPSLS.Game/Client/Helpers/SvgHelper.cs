using System.IO;
using Microsoft.AspNetCore.Components;

namespace RPSLS.Game.Client.Helpers
{
    public class SvgHelper
    {
        public MarkupString GetImage(string imagePath)
        {
            var content = File.ReadAllText("wwwroot" + imagePath);
            return new MarkupString(content);
        }
    }
}
