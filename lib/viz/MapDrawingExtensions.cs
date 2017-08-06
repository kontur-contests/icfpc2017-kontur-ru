using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace lib.viz
{
    public static class MapDrawingExtensions
    {
        public static Map NormalizeCoordinates(this Map map, SizeF targetSize, SizeF padding)
        {
            var normalizeCoordinates = map.Sites
                .Select(s => s.Point())
                .ToArray()
                .NormalizeCoordinates(targetSize, padding);
            var normalizeSites = map.Sites.Zip(normalizeCoordinates, (s, p) => new Site(s.Id, p.X, p.Y)).ToArray();
            return new Map(normalizeSites, map.RiversList, map.Mines, map.OptionsUsed);
        }

        public static IEnumerable<PointF> NormalizeCoordinates(this PointF[] points, SizeF targetSize, SizeF padding)
        {
            var box = points.GetBoundingBox();
            var innerSize = new SizeF(targetSize.Width - 2 * padding.Width, targetSize.Height - 2 * padding.Height);
            return points.Select(p => NormalizeCoordinates(p, box, innerSize))
                .Select(p => new PointF(p.X + padding.Width, p.Y + padding.Height));
        }

        private static PointF NormalizeCoordinates(PointF point, RectangleF bbox, SizeF targetSize)
        {
            return new PointF(
                targetSize.Width * (point.X - bbox.Left) / bbox.Width,
                targetSize.Height * (point.Y - bbox.Top) / bbox.Height);
        }

        public static RectangleF GetBoundingBox(this IEnumerable<PointF> pointsSequence)
        {
            var points = pointsSequence.DefaultIfEmpty(new PointF(0, 0)).ToArray();
            var minX = points.Min(s => s.X);
            var maxX = points.Max(s => s.X);
            var minY = points.Min(s => s.Y);
            var maxY = points.Max(s => s.Y);
            var width = maxX - minX;
            if (width < 1e-7)
                width = 1;
            var height = maxY - minY;
            if (height < 1e-7)
                height = 1;
            return new RectangleF(minX, minY, width, height);
        }

        public static PointF Point(this Site site)
        {
            return new PointF(site.X, site.Y);
        }
    }
}