using Newtonsoft.Json;

namespace lib
{
    public class Setup
    {
        [JsonProperty("punter")]
        public string OurId { get; set; }
        [JsonProperty("punters")]
        public int PunterCount { get; set; }
        [JsonProperty("map")]
        public Map Map { get; set; }
    }
}