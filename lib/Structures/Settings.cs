using Newtonsoft.Json;

namespace lib.Structures
{
    public class Settings
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool futures;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool splurges;

        public Settings()
        {
        }

        public Settings(bool futures, bool splurges)
        {
            this.futures = futures;
            this.splurges = splurges;
        }

        public override string ToString()
        {
            return $"{nameof(futures)}: {futures}, {nameof(splurges)}: {splurges}";
        }
    }
}