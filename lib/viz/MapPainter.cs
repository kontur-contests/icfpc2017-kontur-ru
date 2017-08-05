using System.Diagnostics;
using System.Drawing;
using System.Linq;
using lib.viz.Detalization;
using MoreLinq;

namespace lib.viz
{
    public class MapPainter : IScenePainter
    {
        private static readonly Font font = new Font(FontFamily.GenericSansSerif, 6);
        private IndexedMap map;
        private IPainterAugmentor painterAugmentor = new DefaultPainterAugmentor();

        public Map Map
        {
            get => map.Map;
            set
            {
                map = new IndexedMap(value.NormalizeCoordinates(Size, Padding));
                if (painterAugmentor != null)
                    painterAugmentor.Map = map;
            }
        }

        public IPainterAugmentor PainterAugmentor
        {
            get => painterAugmentor;
            set
            {
                painterAugmentor = value;
                if (map != null)
                    painterAugmentor.Map = map;
            }
        }

        private static SizeF Padding => new SizeF(30, 30);
        public SizeF Size => new SizeF(600, 600);
        private GameState gameState;

        public GameState GameState
        {
            get { return gameState; }
            set
            {
                gameState = value;
                if (gameState != null)
                    Map = gameState.CurrentMap;
            }
        }

        public void Paint(Graphics g, PointF mouseLogicalPos, RectangleF clipRect)
        {
            if (map == null) return;
            var sw = Stopwatch.StartNew();
                HightlightLastMove(g, gameState?.PreviousMoves?.LastOrDefault());
            foreach (var river in map.Rivers)
                DrawRiver(g, river);
            foreach (var site in map.Sites)
                DrawSite(g, site);
            DrawRiverText(g, map.Rivers.MinBy(r => DistanceTo(mouseLogicalPos, r)));
            g.DrawString(sw.Elapsed.TotalMilliseconds.ToString("0ms"), SystemFonts.DefaultFont, Brushes.Black, PointF.Empty);
        }

        private void HightlightLastMove(Graphics g, Move move)
        {
            if (move is ClaimMove m)
            {
                var start = map.SiteById[m.Source];
                var end = map.SiteById[m.Target];
                var radius = 7;
                using (var pen = new Pen(Color.GreenYellow, radius))
                {
                    var center = 0.5f*(new VF(start.Point()) + new VF(end.Point())) - new VF(radius, radius);
                    g.DrawEllipse(pen, (float) center.X, (float)center.Y, 2*radius, 2 * radius);
                }
            }
        }

        private void DrawRiver(Graphics g, River river)
        {
            var data = PainterAugmentor.GetData(river);
            var source = map.SiteById[river.Source];
            var target = map.SiteById[river.Target];
            using (var pen = new Pen(data.Color, data.PenWidth))
            {
                g.DrawLine(pen, source.Point(), target.Point());
            }
        }

        private void DrawSite(Graphics g, Site site)
        {
            var data = PainterAugmentor.GetData(site);
            var radius = data.Radius;
            var rectangle = new RectangleF(site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
            using (var brush = new SolidBrush(data.Color))
            {
                if (map.MineIds.Contains(site.Id))
                    g.FillRectangle(brush, rectangle);
                else
                    g.FillEllipse(brush, rectangle);
            }
        }

        private void DrawSiteText(Graphics g, Site site)
        {
            var data = PainterAugmentor.GetData(site);
            var radius = data.Radius;
            var drawPoint = new PointF(site.X - radius - 7, site.Y - radius - 7);
            g.DrawString(data.HoverText, font, Brushes.Black, drawPoint);
        }

        private void DrawRiverText(Graphics g, River river)
        {
            var data = PainterAugmentor.GetData(river);
            var sourceSite = map.SiteById[river.Source];
            var targetSite = map.SiteById[river.Target];
            var start = new VF(sourceSite.Point());
            var end = new VF(targetSite.Point());
            var drawPoint = ((start + end) * 0.5).Translate(-5, 0).ToPointF;
            var stringBoxSize = g.MeasureString(data.HoverText, font, drawPoint, StringFormat.GenericDefault);
            using (var pen = new Pen(data.Color, 3 * data.PenWidth))
            {
                g.DrawLine(pen, start.ToPointF, end.ToPointF);
            }
            using (var textBckgPen = new SolidBrush(Color.FromArgb(192, Color.White)))
            {
                g.FillRectangle(textBckgPen, new RectangleF(drawPoint, stringBoxSize));
            }
            DrawSite(g, sourceSite);
            DrawSite(g, targetSite);
            DrawSiteText(g, sourceSite);
            DrawSiteText(g, targetSite);
            g.DrawString(data.HoverText, font, Brushes.Black, drawPoint);
        }

        private double DistanceTo(PointF cursor, River river)
        {
            var point = new VF(cursor);
            var start = new VF(map.SiteById[river.Source].Point());
            var end = new VF(map.SiteById[river.Target].Point());
            var v = end - start;
            var w = point - start;
            var c1 = w.ScalarProd(v);
            var c2 = v.ScalarProd(v);
            if (c1 <= 0)
                return (start - point).LengthSquared();
            if (c2 <= c1)
                return (end - point).LengthSquared();
            var b = c1 / c2;
            var pb = start + b * v;
            return (point - pb).LengthSquared();
        }
    }
}