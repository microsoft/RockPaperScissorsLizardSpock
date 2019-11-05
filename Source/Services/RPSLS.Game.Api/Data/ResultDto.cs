using Newtonsoft.Json;
using RPSLS.Game.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPSLS.Game.Api.Data
{

    public class ResultDto 
    {
        [JsonProperty("value")]
        public int Value {get; set;}
        [JsonProperty("winner")]
        public string Winner {get; set;}
    }
    public class MoveDto 
    {
        [JsonProperty("value")]
        public int Value {get; set;}
        [JsonProperty("text")]
        public string Text {get; set;}
    }
    public class ChallengerDto
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    public class MatchDto
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; }

        [JsonProperty(PropertyName = "whenUtc")]
        public DateTime WhenUtc { get; }

        [JsonProperty(PropertyName = "challenger")]
        public ChallengerDto Challenger { get; }
        [JsonProperty(PropertyName = "playerName")]
        public string PlayerName { get; set; }
        [JsonProperty(PropertyName = "playerLogged")]
        public bool PlayerLogged { get; set; }

        [JsonProperty(PropertyName = "playerMove")]
        public MoveDto PlayerMove {get; set;}

        [JsonProperty(PropertyName = "challengerMove")]
        public MoveDto ChallengerMove {get; set;}

        [JsonProperty(PropertyName = "result")]
        public ResultDto Result {get; set;}        

        public MatchDto()
        {
            Id = Guid.NewGuid();
            Challenger = new ChallengerDto();
            WhenUtc = DateTime.UtcNow;
            PlayerMove = new  MoveDto();
            ChallengerMove = new MoveDto();
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
    }
}
