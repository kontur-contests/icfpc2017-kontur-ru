using System.Drawing;
using lib.viz.Detalization;

namespace lib.viz
{
    public class MapPainter : IScenePainter
    {
        private IndexedMap map;

        public Map Map
        {
            get => map.Map;
            set => map = new IndexedMap(value.NormalizeCoordinates(Size, Padding));
        }

        public IPainterAugmentor PainterAugmentor { get; set; } = new DefaultPainterAugmentor();

        private static SizeF Padding => new SizeF(30, 30);
        public SizeF Size => new SizeF(600, 600);

        public void Paint(Graphics g)
        {
            g.Clear(Color.White);

            foreach (var river in map.Rivers)
                DrawRiver(g, river);
            foreach (var site in map.Sites)
                DrawSite(g, site);
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
            using (var brush = new SolidBrush(data.Color))
            {
                var rectangle = new RectangleF(site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
                if(map.MineIds.Contains(site.Id))
                    g.FillRectangle(brush, rectangle);
                else
                    g.FillEllipse(brush, rectangle);
            }

            //g.DrawEllipse(Pens.Black, site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
        }
    }
}