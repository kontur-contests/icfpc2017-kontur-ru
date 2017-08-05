using Newtonsoft.Json;

namespace lib
{
    public class Settings
    {
        [JsonProperty("futures")] public bool Futures;

        public Settings()
        {
        }

        public Settings(bool futures)
        {
            Futures = futures;
        }
    }
}