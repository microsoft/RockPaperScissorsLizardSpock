using Newtonsoft.Json;

namespace RPSLS.Game.Api.Data.Models
{
    public class ResultDto
    {
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("winner")]
        public string Winner { get; set; }
    }
}
