using System;
using System.Drawing;
using System.Linq;

namespace lib.viz.Detalization
{
    public class DefaultPainterAugmentor : IPainterAugmentor
    {
        private IndexedMap map;
        private float radius;
        private const float defaultRadius = 30f;

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
                Radius = radius,
                HoverText = site.Id.ToString(),
            };
        }

        public RiverPainterData GetData(River river)
        {
            return new RiverPainterData
            {

                Color = river.Owner == -1 ? Color.Blue : Color.FromKnownColor((KnownColor) river.Owner) ,
                PenWidth = 1,
                HoverText = "It's a river!"
            };
        }

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