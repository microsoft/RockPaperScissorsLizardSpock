using RPSLS.DotNetPlayer.Api.Models;
using System;
using System.Net;

namespace RPSLS.DotNetPlayer.API
{
    public class Choice
    {
        public string PlayerType { get; set; } = "dotnet";
        public string Player { get; set; } = Dns.GetHostName();
        public int Value { get; set; }
        public string Text { get; set; }

        public Choice(int value) : this((RPSLSEnum)value) { }
        public Choice(RPSLSEnum value)
        {
            this.Value = (int) value;
            this.Text = Enum.GetName(typeof(RPSLSEnum), value);
        }
    }
}
