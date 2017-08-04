using System;
using System.Drawing;
using System.IO;
using lib;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace CinemaLib
{
    public class MapPainter : IScenePainter
    {
        private Map map;

        public Map Map
        {
            get => map;
            set => map = value.NormalizeCoordinates(Size, Padding);
        }

        private static SizeF Padding => new SizeF(30, 30);
        public SizeF Size => new SizeF(600, 600);

        public void Paint(Graphics g)
        {
            g.Clear(Color.White);
            foreach (var river in map.Rivers)
                g.DrawLine(Pens.Blue, map.Sites[river.Source].Point(), map.Sites[river.Target].Point());
            foreach (var site in map.Sites)
                DrawSite(g, site);
        }

        private void DrawSite(Graphics g, Site site)
        {
            var radius = 15;
            g.FillEllipse(GetSiteColor(site), site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
            g.DrawEllipse(Pens.Black, site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
        }

        private Brush GetSiteColor(Site site)
        {
            return map.IsMine(site.Id) ? Brushes.Red : Brushes.LimeGreen;
        }
    }

    [TestFixture]
    public class MapPainter_Should
    {
        [Test]
        [STAThread]
        [Explicit]
        public void Show()
        {
            var form = new Form();
            var painter = new MapPainter();
            painter.Map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\circle.json")).Map;
            var panel = new ScaledViewPanel(painter)
            {
                Dock = DockStyle.Fill
            };
            form.Controls.Add(panel);
            form.ShowDialog();
        }
    }
}