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
        public static Map NormalizeCoordinates(this Map map, SizeF targetSize)
        {
            var box = map.GetBoundingBox();
            return new Map(
                map.Sites.Select(s => NormalizeCoordinates(s, box, targetSize)).ToArray(), map.Rivers, map.Mines);
        }

        private static Site NormalizeCoordinates(Site site, RectangleF bbox, SizeF targetSize)
        {
            return new Site(
                site.Id, targetSize.Width * (site.X - bbox.Left) / bbox.Width,
                targetSize.Height * (site.Y - bbox.Top) / bbox.Height);
        }

        public static RectangleF GetBoundingBox(this Map map)
        {
            float minX = map.Sites.Min(s => s.X);
            float maxX = map.Sites.Max(s => s.X);
            float minY = map.Sites.Min(s => s.Y);
            float maxY = map.Sites.Max(s => s.Y);
            return new RectangleF(minX, minY, Math.Max(1, maxX - minX), Math.Max(1, maxY - minY));
        }

        public static bool IsMine(this Map map, int siteId)
        {
            return map.Mines.Contains(siteId);
        }
    }
}