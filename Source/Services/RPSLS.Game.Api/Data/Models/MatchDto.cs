using Newtonsoft.Json;
using RPSLS.Game.Api.Services;
using System;
using ProtoResult = GameApi.Proto.Result;

namespace RPSLS.Game.Api.Data.Models
{
    public class MatchDto
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "whenUtc")]
        public DateTime WhenUtc { get; set; }

        [JsonProperty(PropertyName = "challenger")]
        public ChallengerDto Challenger { get; set; }

        [JsonProperty(PropertyName = "playerName")]
        public string PlayerName { get; set; }

        [JsonProperty(PropertyName = "playerMove")]
        public MoveDto PlayerMove { get; set; }

        [JsonProperty(PropertyName = "challengerMove")]
        public MoveDto ChallengerMove { get; set; }

        [JsonProperty(PropertyName = "result")]
        public ResultDto Result { get; set; }

        [JsonProperty(PropertyName = "playFabMatchId")]
        public string PlayFabMatchId { get; set; }

        public MatchDto()
        {
            Id = Guid.NewGuid();
            PlayerMove = new MoveDto();
            Challenger = new ChallengerDto();
            ChallengerMove = new MoveDto();
            WhenUtc = DateTime.UtcNow;
            Result = new ResultDto();
        }

        public static MatchDto FromPickDto(PickDto pick)
        {
            var dto = new MatchDto();
            dto.Challenger.Name = pick.Player;
            dto.Challenger.Type = pick.PlayerType;
            dto.ChallengerMove.Text = pick.Text;
            dto.ChallengerMove.Value = pick.Value;
            return dto;
        }

        public static MatchDto CreateMissing(string matchId)
        {
            var dto = new MatchDto();
            dto.Challenger.Name = "-";
            dto.Challenger.Type = "human";
            dto.PlayFabMatchId = matchId;
            dto.PlayerName = "-";
            dto.Result.Winner = Enum.GetName(typeof(ProtoResult), ProtoResult.Challenger);
            dto.Result.Value = (int) ProtoResult.Challenger;
            return dto;
        }
    }
}
