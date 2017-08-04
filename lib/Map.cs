using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace lib
{
    public class Site
    {
        [JsonProperty("id")] public int Id;

        [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float X;

        [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float Y;

        public Site()
        {
        }

        public Site(int id, float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }

    public class Map
    {
        [JsonProperty("mines", Order = 3)] public int[] Mines;

        [JsonProperty("rivers", Order = 2)] public River[] Rivers;

        [JsonProperty("sites", Order = 1)] public Site[] Sites;

        public Map()
        {
        }

        public Map(Site[] sites, River[] rivers, int[] mines)
        {
            Sites = sites;
            Rivers = rivers;
            Mines = mines;
        }
    }

    public class River
    {
        [JsonProperty("source", Order=1)] public readonly int Source;

        [JsonProperty("target", Order = 2)] public readonly int Target;

        [JsonIgnore] public int Owner;

        public River()
        {
        }

        public River(int source, int target, int owner = -1)
        {
            Source = source;
            Target = target;
            Owner = owner;
        }
    }

    public static class MapExtensions
    {
        public static bool IsMine(this Map map, int siteId)
        {
            return map.Mines.Contains(siteId);
        }
    }
}