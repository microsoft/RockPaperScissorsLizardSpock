using Microsoft.AspNetCore.Components;
using System.IO;

namespace RPSLS.Web.Helpers
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
