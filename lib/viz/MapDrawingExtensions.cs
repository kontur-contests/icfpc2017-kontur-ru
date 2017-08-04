using System;
using System.Drawing;
using System.Linq;
using lib;

namespace CinemaLib
{
    public static class MapDrawingExtensions
    {
        public static Map NormalizeCoordinates(this Map map, SizeF targetSize, SizeF padding)
        {
            var box = map.GetBoundingBox();
            padding = new SizeF(padding.Width * box.Width / targetSize.Width, padding.Height * box.Height / targetSize.Height);
            var paddedBox = new RectangleF(box.X - padding.Width, box.Y - padding.Height, box.Width + 2 * padding.Width, box.Height + 2 * padding.Height);
            return new Map(
                map.Sites.Select(s => NormalizeCoordinates(s, paddedBox, targetSize)).ToArray(), map.Rivers, map.Mines);
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