using Newtonsoft.Json;

namespace lib.Structures
{
    public class Settings
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool futures;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool splurges;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool options;

        public Settings()
        {
        }

        public Settings(bool futures, bool splurges, bool options)
        {
            this.futures = futures;
            this.splurges = splurges;
            this.options = options;
        }

        public override string ToString()
        {
            return $"{nameof(futures)}: {futures}, {nameof(splurges)}: {splurges}, {nameof(options)}: {options}";

        }
    }
}