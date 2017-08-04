using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lib;
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
            {
                var source = map.Sites.Single(x => x.Id == river.Source);
                var target = map.Sites.Single(x => x.Id == river.Target);
                g.DrawLine(Pens.Blue, source.Point(), target.Point());
            }
            foreach (var site in map.Sites)
                DrawSite(g, site);
        }

        private void DrawSite(Graphics g, Site site)
        {
            var radius = 3;
            g.FillEllipse(GetSiteColor(site), site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
            //g.DrawEllipse(Pens.Black, site.X - radius, site.Y - radius, 2 * radius, 2 * radius);
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
            painter.Map = MapLoader.LoadMap(
                    Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\tube.json"))
                .Map;
            var panel = new ScaledViewPanel(painter)
            {
                Dock = DockStyle.Fill
            };
            form.Controls.Add(panel);
            form.ShowDialog();
        }
    }
}