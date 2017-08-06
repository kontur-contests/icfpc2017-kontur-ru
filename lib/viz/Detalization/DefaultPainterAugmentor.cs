using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using lib.GraphImpl;
using lib.Structures;

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
        private readonly Dictionary<int, ConnectedCalculator> connectedCalcs = new Dictionary<int, ConnectedCalculator>();

        private IndexedMap map;
        private Graph graph;
        private float radius;

        public IndexedMap Map
        {
            get => map;
            set
            {
                map = value;
                radius = Math.Max(0.5f, Math.Min(defaultRadius, CalcMinDistance() / 5));
                connectedCalcs.Clear();
                graph = new Graph(map.Map);
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
            var data = new RiverPainterData
            {
                Color = FadeBySelectedOwner(river.Owner == -1 ? Color.LightGray : Colors[river.Owner], river.Owner),
                OptionColor = FadeBySelectedOwner(river.OptionOwner == -1 ? Color.Transparent : Colors[river.OptionOwner], river.OptionOwner),
                PenWidth = river.Owner == -1 ? 1 : 3,
                HoverText = river.Owner >= 0 ? "Owner " + river.Owner : "",
            };
            return data;
        }

        private Color FadeBySelectedOwner(Color color, int riverOwner)
        {
            if (riverOwner != SelectedPlayerIndex && SelectedPlayerIndex != -1)
            {
                return Color.FromArgb(30, color);
            }
            return color;
        }

        public FuturePainterData GetData(int punderId, Future future)
        {
            if (!connectedCalcs.ContainsKey(punderId))
                connectedCalcs.Add(punderId, new ConnectedCalculator(graph, punderId));
            var calc = connectedCalcs[punderId];
            var conected = calc.GetConnectedMines(future.source).Contains(future.target) ||
                           calc.GetConnectedMines(future.target).Contains(future.source);
            return new FuturePainterData
            {
                Color = FadeBySelectedOwner(Colors[punderId], punderId),
                PenWidth = conected ? 1 : 2,
                DashStyle = DashStyle.Solid
            };
        }

        public bool ShowFutures { get; set; }
        public int SelectedPlayerIndex { get; set; } = -1;

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