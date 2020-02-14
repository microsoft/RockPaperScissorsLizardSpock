using Newtonsoft.Json;

namespace RPSLS.Game.Api.Data.Models
{
    public class MoveDto
    {
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
