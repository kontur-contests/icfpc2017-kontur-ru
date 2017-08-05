using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace lib.viz.Detalization
{
    public class ColorsPalette
    {
        public static Color[] Colors =
        {
            Color.Navy,
            Color.Red,
            Color.Orange,
            Color.Fuchsia,
            Color.Green,
            Color.Brown,
            Color.CornflowerBlue,
            Color.CadetBlue,
            Color.DimGray,
            Color.Chocolate,
            Color.BlueViolet,
            Color.LightSeaGreen,
            Color.DeepPink,
            Color.MediumOrchid,
            Color.MidnightBlue,
            Color.Teal,
            Color.Sienna,
            Color.SlateBlue,
        };

    }
    public class DefaultPainterAugmentor : IPainterAugmentor
    {
        private const float defaultRadius = 30f;
        private static Color[] Colors = ColorsPalette.Colors;

        private IndexedMap map;
        private float radius;

        public IndexedMap Map
        {
            get => map;
            set
            {
                map = value;
                radius = Math.Max(0.5f, Math.Min(defaultRadius, CalcMinDistance() / 5));
            }
        }

        public SitePainterData GetData(Site site)
        {
            return new SitePainterData
            {
                Color = GetSiteColor(site),
                Radius = map.MineIds.Contains(site.Id) ? Math.Min(radius * 5, defaultRadius) : radius,
                HoverText = site.Id.ToString(),
            };
        }

        public RiverPainterData GetData(River river)
        {
            return new RiverPainterData
            {
                Color = river.Owner == -1 ? Color.LightGray : Colors[river.Owner],
                PenWidth = river.Owner == -1 ? 1 : 3,
                HoverText = "It's a river!",
                DashStyle = DashStyle.Solid
            };
        }

        public FuturePainterData GetData(int punderId, Future future)
        {
            return new FuturePainterData
            {
                Color = Colors[punderId],
                PenWidth = 5
            };
        }

        public bool ShowFutures { get; set; }

        private float CalcMinDistance()
        {
            return (float) Map.Rivers.Min(
                river =>
                {
                    var s1 = map.SiteById[river.Source];
                    var t1 = map.SiteById[river.Target];
                    var dx1 = t1.X - s1.X;
                    var dy1 = t1.Y - s1.Y;
                    var dist = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
                    return dist < 1e-5 ? defaultRadius : dist;
                });
        }

        private Color GetSiteColor(Site site)
        {
            return map.MineIds.Contains(site.Id) ? Color.Red : Color.LimeGreen;
        }
    }
}