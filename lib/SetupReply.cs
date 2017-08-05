using Newtonsoft.Json;

namespace lib
{
    public class SetupReply
    {
        [JsonProperty("ready")] public int Id;

        [JsonProperty("futures")] public Future[] Futures;

        public SetupReply()
        {
        }

        public SetupReply(int id, Future[] futures)
        {
            Id = id;
            Futures = futures;
        }
    }
}