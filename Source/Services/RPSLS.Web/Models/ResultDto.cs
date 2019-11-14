namespace RPSLS.Web.Models
{
    public class ResultDto
    {
        public string Challenger { get; set; }
        public int ChallengerPick { get; set; }
        public string User { get; set; }
        public int UserPick { get; set; }
        public int Result { get; set; }
        public bool IsValid { get; set; }
    }
}
