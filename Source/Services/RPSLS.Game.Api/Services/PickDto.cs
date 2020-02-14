namespace RPSLS.Game.Api.Services
{
    public class PickDto
    {
        public int Value { get; set; }
        public string Text { get; set; }
      
        public string PlayerType { get; set; }
        public string Player { get; set; }

        internal static string ToText(int pick)
        {
            return pick switch
            {
                0 => "Rock",
                1 => "Paper",
                2 => "Scissors",
                3 => "Lizard",
                4 => "Spock",
                _ => "Unknown",
            };
        }
    }
}
