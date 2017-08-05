using Newtonsoft.Json;

namespace lib
{
    public class Setup
    {
        [JsonProperty("punter")] public int Id;

        [JsonProperty("punters")] public int PunterCount;

        [JsonProperty("map")] public Map Map;

        [JsonProperty("settings")] public Settings Settings;

        public Setup()
        {
            Settings = new Settings();
        }

        public Setup(int id, int punterCount, Map map, Settings settings)
        {
            Id = id;
            PunterCount = punterCount;
            Map = map;
            Settings = settings;
        }
    }
}