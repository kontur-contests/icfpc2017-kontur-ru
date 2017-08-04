using System.Diagnostics;
using System.Drawing;
using System.Linq;
using lib.viz.Detalization;

namespace lib.viz
{
    public class MapPainter : IScenePainter
    {
        private static readonly Font font = new Font(FontFamily.GenericSansSerif, 6);
        private IndexedMap map;

        public Map Map
        {
            get => map.Map;
            set => map = new IndexedMap(value.NormalizeCoordinates(Size, Padding));
        }

        public IPainterAugmentor PainterAugmentor { get; set; } = new DefaultPainterAugmentor();

        private static SizeF Padding => new SizeF(30, 30);
        public SizeF Size => new SizeF(600, 600);

        public void Paint(Graphics g, PointF mouseLogicalPos, RectangleF clipRect)
        {
            var sw = Stopwatch.StartNew();
            foreach (var river in map.Rivers)
                    DrawRiver(g, river);
            foreach (var site in map.Sites)
                DrawSite(g, site);
            foreach (var site in map.Sites)
                DrawSiteText(g, site, mouseLogicalPos);
            g.DrawString(sw.Elapsed.TotalMilliseconds.ToString("0ms"), SystemFonts.DefaultFont, Brushes.Black, PointF.Empty);
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

        private void DrawSiteText(Graphics g, Site site, PointF mouseLogicalPos)
        {
            var data = PainterAugmentor.GetData(site);
            var radius = data.Radius;
            var rectangle = new RectangleF(site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
            if (rectangle.Contains(mouseLogicalPos))
                g.DrawString(site.Id.ToString(), font, Brushes.Black, RectangleF.Inflate(rectangle, 7, 7).Location);
        }
    }
}