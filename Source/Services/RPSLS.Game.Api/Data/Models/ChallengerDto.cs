using Newtonsoft.Json;

namespace RPSLS.Game.Api.Data.Models
{
    public class ChallengerDto
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}
