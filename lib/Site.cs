using Newtonsoft.Json;

namespace lib
{
    public class Site
    {
        [JsonProperty("id")] public int Id;

        [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)] public float X;

        [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)] public float Y;

        public Site()
        {
        }

        public Site(int id, float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }
    }
}