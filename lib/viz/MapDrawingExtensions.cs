using System;
using System.Drawing;
using System.Linq;
using lib;

namespace CinemaLib
{
    public static class MapDrawingExtensions
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

        public static PointF Point(this Site site)
        {
            return new PointF(site.X, site.Y);
        }
    }
}