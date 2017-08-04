using System;
using System.IO;
using System.Windows.Forms;
using lib.viz.Detalization;
using NUnit.Framework;

namespace lib.viz
{
    [TestFixture]
    public class MapPainter_Should
    {
        [Test]
        [STAThread]
        [Explicit]
        public void Show()
        {
            var form = new Form();
            var map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\circle.json")).Map;
            var painter = new MapPainter
            {
                Map = map,
                PainterAugmentor = new DefaultPainterAugmentor()
            };
            var panel = new ScaledViewPanel(painter)
            {
                Dock = DockStyle.Fill
            };
            form.Controls.Add(panel);
            form.ShowDialog();
        }
    }
}