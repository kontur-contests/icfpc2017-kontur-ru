using Newtonsoft.Json;

namespace lib.Structures
{
    public class Settings
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool futures;

        public Settings()
        {
        }

        public Settings(bool futures)
        {
            this.futures = futures;
        }

        public override string ToString()
        {
            return $"{nameof(futures)}: {futures}";
        }
    }
}