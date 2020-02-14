namespace RPSLS.Game.Multiplayer.Models
{
    public class MatchResult
    {
        public string Status { get; set; }
        public string MatchId { get; set; }
        public string Opponent { get; set; }

        public bool Finished { get => !string.IsNullOrWhiteSpace(Status) && !Status.StartsWith("Waiting"); }
        public bool Matched { get => "Matched".Equals(Status); }
    }
}
