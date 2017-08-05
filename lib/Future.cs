using Newtonsoft.Json;

namespace lib
{
    public class Future
    {
        [JsonProperty("source")] public int Source;

        [JsonProperty("target")] public int Target;

        public Future()
        {
        }

        public Future(int source, int target)
        {
            Source = source;
            Target = target;
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}";
        }
    }
}