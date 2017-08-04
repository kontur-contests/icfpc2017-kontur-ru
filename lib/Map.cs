using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NLog.Layouts;

namespace lib
{
    public class Site
    {
        public Site()
        {
        }

        public Site(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        [JsonProperty("id")]
        public int Id;
        [JsonIgnore]
        public double X;
        [JsonIgnore]
        public double Y;

    }
    public class Map
    {
        public Map()
        {
        }

        public Map(Site[] sites, River[] rivers, int[] mines)
        {
            Sites = sites;
            Rivers = rivers;
            Mines = mines;
        }

        [JsonProperty("sites")]
        public Site[] Sites;
        [JsonProperty("rivers")]
        public River[] Rivers;
        [JsonProperty("mines")]
        public int[] Mines;
    }

    public class River
    {
        public River()
        {
        }

        public River(int source, int target, int owner = -1)
        {
            Source = source;
            Target = target;
            Owner = owner;
        }

        [JsonProperty("source")]
        public readonly int Source;
        [JsonProperty("target")]
        public readonly int Target;
        [JsonIgnore]
        public int Owner;
    }
}


