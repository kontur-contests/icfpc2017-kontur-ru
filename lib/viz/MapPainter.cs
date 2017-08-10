using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using lib.Structures;
using lib.viz.Detalization;
using MoreLinq;

namespace lib.viz
{
    public class MapPainter : IScenePainter
    {
        private Dictionary<int, Future[]> futures;
        private GameState gameState;
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

        public Dictionary<int, Future[]> Futures
        {
            get => futures ?? new Dictionary<int, Future[]>();
            set => futures = value;
        }

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

        public SizeF Size => new SizeF(600, 600);

        public void Paint(Graphics g, PointF mouseLogicalPos, RectangleF clipRect, float zoomScale)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            zoomScale /= 1.5f;
            if (map == null) return;
            var sw = Stopwatch.StartNew();
            //HightlightLastMove(g, gameState?.PreviousMoves?.LastOrDefault(), zoomScale);
            foreach (var river in map.Rivers)
                DrawRiver(g, river, zoomScale);
            foreach (var site in map.Sites)
                DrawSite(g, site);
            if (PainterAugmentor.ShowFutures)
                foreach (var futuresGroup in Futures)
                foreach (var future in futuresGroup.Value)
                    DrawFuture(g, futuresGroup.Key, future);
            DrawRiverText(g, map.Rivers.MinBy(r => DistanceTo(mouseLogicalPos, r)), zoomScale);
            g.DrawString(sw.Elapsed.TotalMilliseconds.ToString("0ms"), SystemFonts.DefaultFont, Brushes.Black, PointF.Empty);
        }

        private void HightlightLastMove(Graphics g, Move move, float zoomScale)
        {
            var claims = new List<int>();
            if (move?.claim != null)
            {
                claims.Add(move.claim.source);
                claims.Add(move.claim.target);
            }
            else if (move?.splurge != null)
            {
                claims.AddRange(move.splurge.route);
            }
            else if (move?.option != null)
            {
                claims.Add(move.option.source);
                claims.Add(move.option.target);
            }

            if (claims.Count >= 2)
            {
                var old = claims[0];
                foreach (var vertex in claims.Skip(1))
                {
                    var start = map.SiteById[old];
                    var end = map.SiteById[vertex];
                    old = vertex;
                    var radius = 7 / zoomScale;
                    using (var pen = new Pen(Color.GreenYellow, radius))
                    {
                        var center = 0.5f * (new VF(start.Point()) + new VF(end.Point())) - new VF(radius, radius);
                        g.DrawEllipse(pen, (float)center.X, (float)center.Y, 2 * radius, 2 * radius);
                    }
                }
            }
        }

        private void DrawRiver(Graphics g, River river, float zoomScale)
        {
            var data = PainterAugmentor.GetData(river);
            var source = map.SiteById[river.Source];
            var target = map.SiteById[river.Target];
            DrawDiverWithPenWidth(g, zoomScale, river, data, source.Point(), target.Point());
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

        private void DrawFuture(Graphics g, int punderId, Future future)
        {
            var data = PainterAugmentor.GetData(punderId, future);
            var source = map.SiteById[future.source];
            var target = map.SiteById[future.target];
            var pen = new Pen(data.Color, data.PenWidth)
            {
                StartCap = LineCap.ArrowAnchor,
                EndCap = LineCap.RoundAnchor,
                DashStyle = data.DashStyle
            };
            using (pen)
            {
                g.DrawBezier(pen, source.Point(), target.Point(), punderId * 3 + 12);
            }
        }

        private void DrawSiteText(Graphics g, Site site, Font font)
        {
            var data = PainterAugmentor.GetData(site);
            var radius = data.Radius;
            var drawPoint = new PointF(site.X - radius - 7, site.Y - radius - 7);
            g.DrawString(data.HoverText, font, Brushes.Black, drawPoint);
        }

        private void DrawRiverText(Graphics g, River river, float zoomScale)
        {
            var data = PainterAugmentor.GetData(river);
            var sourceSite = map.SiteById[river.Source];
            var targetSite = map.SiteById[river.Target];
            var start = new VF(sourceSite.Point());
            var end = new VF(targetSite.Point());
            var drawPoint = ((start + end) * 0.5).Translate(-5, 0).ToPointF;

            DrawDiverWithPenWidth(g, zoomScale, river, data, start.ToPointF, end.ToPointF, 3);
            using (var font = new Font(FontFamily.GenericSansSerif, 6 / zoomScale))
            {
                var stringBoxSize = g.MeasureString(data.HoverText, font, drawPoint, StringFormat.GenericDefault);
                using (var textBckgPen = new SolidBrush(Color.FromArgb(192, Color.White)))
                {
                    g.FillRectangle(textBckgPen, new RectangleF(drawPoint, stringBoxSize));
                }
                DrawSite(g, sourceSite);
                DrawSite(g, targetSite);
                DrawSiteText(g, sourceSite, font);
                DrawSiteText(g, targetSite, font);
                g.DrawString(data.HoverText, font, Brushes.Black, drawPoint);
            }
        }

        private static void DrawDiverWithPenWidth(Graphics g, float zoomScale, River river, RiverPainterData data, PointF source, PointF target, float penWidthMultiplier = 1)
        {
            using (var pen = new Pen(data.Color, penWidthMultiplier * data.PenWidth / zoomScale) { DashStyle = DashStyle.Solid })
            using (var optionPen = new Pen(data.OptionColor, penWidthMultiplier * 2 * data.PenWidth / zoomScale) { DashStyle = DashStyle.Dash, DashPattern = new[] { 2f/ penWidthMultiplier, 2f/penWidthMultiplier } })
            {
                g.DrawLine(pen, source, target);
                if (river.OptionOwner != -1)
                    g.DrawLine(optionPen, source, target);
            }
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

    public static class ArcDrawerHelper
    {
        private const float Epsilon = 0.0001f;

        public static void DrawBezier(this Graphics g, Pen pen, PointF source, PointF target, int radius)
        {
            var mid = FindMidPoints(source, target, radius);
            g.DrawBezier(pen, source, mid.Item1, mid.Item2, target);
        }

        private static Tuple<PointF, PointF> FindMidPoints(PointF f, PointF s, float distance)
        {
            var abc = FindABC(f, s);
            var a = abc.Item1;
            var b = abc.Item2;
            var n = new SizeF(a, b).Normalize();
            var d = new SizeF(s.X - f.X, s.Y - f.Y);
            var fc = f + d.Mul(1f / 3);
            var sc = f + d.Mul(2f / 3);
            return Tuple.Create(fc + n.Mul(distance), sc + n.Mul(distance));
        }

        private static Tuple<float, float, float> FindABC(PointF f, PointF s)
        {
            var dx = f.X - s.X;
            var dy = f.Y - s.Y;
            if (Math.Abs(dx) < Epsilon && Math.Abs(dy) < Epsilon)
                return null;
            if (Math.Abs(dx) < Epsilon)
            {
                return Tuple.Create(1f, 0f, f.X); //vertical
            }
            else if (Math.Abs(dy) < Epsilon)
            {
                return Tuple.Create(0f, 1f, f.Y); //horizontal
            }
            else
            {
                var k = dy / dx;
                var c = k * f.X - f.Y;
                return Tuple.Create(-k, 1f, c);
            }
        }

        private static SizeF Normalize(this SizeF size)
        {
            var l = Math.Sqrt(size.Height * size.Height + size.Width * size.Width);
            return Math.Abs(l) < Epsilon ? size : size.Mul(1f / (float) l);
        }

        private static SizeF Mul(this SizeF size, float k)
        {
            return new SizeF(size.Width * k, size.Height * k);
        }
    }
}