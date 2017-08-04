using System.Linq;
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

    public class Map
    {
        [JsonProperty("mines", Order = 3)] public int[] Mines = new int[0];

        [JsonProperty("rivers", Order = 2)] public River[] Rivers = new River[0];

        [JsonProperty("sites", Order = 1)] public Site[] Sites = new Site[0];

        public Map()
        {
        }

        public Map(Site[] sites, River[] rivers, int[] mines)
        {
            Sites = sites;
            Rivers = rivers;
            Mines = mines;
        }

        public Map Clone()
        {
            return new Map(Sites, Rivers.Select(r => r.Clone()).ToArray(), Mines);
        }
    }

    public class River
    {
        [JsonProperty("source", Order = 1)] public readonly int Source;

        [JsonProperty("target", Order = 2)] public readonly int Target;

        [JsonIgnore] public int Owner = -1;

        public River()
        {
        }

        public River(int source, int target, int owner = -1)
        {
            Source = source;
            Target = target;
            Owner = owner;
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(Target)}: {Target}, {nameof(Owner)}: {Owner}";
        }

        public River Clone()
        {
            return new River(Source, Target, Owner);
        }
    }
}