using System;
using System.Drawing;
using System.IO;
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
            set
            {
                map = value.NormalizeCoordinates(Size);
            }
        }

        public SizeF Size => new SizeF(600, 600);

        public void Paint(Graphics g)
        {
            g.Clear(Color.White);
            var radius = 5;
            foreach (var site in map.Sites)
                g.FillEllipse(Brushes.Blue, site.X - radius, site.Y-radius, 2*radius, 2*radius);
            foreach (int mapMine in map.Mines)
            {
                
            }
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