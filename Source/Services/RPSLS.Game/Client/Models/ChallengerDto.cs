using GameBff.Proto;

namespace RPSLS.Game.Client.Models
{
    public class ChallengerDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public static ChallengerDto FromProtoResponse(ChallengerInfo challengerInfo) => new ChallengerDto()
        {
            Name = challengerInfo.Name,
            DisplayName = challengerInfo.DisplayName
        };
    }
}
