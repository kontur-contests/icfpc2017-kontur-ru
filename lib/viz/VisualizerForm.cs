using System;
using System.Linq;
using System.Windows.Forms;
using lib.viz.Detalization;
using NUnit.Framework;

namespace lib.viz
{
    public class VisualizerForm : Form
    {
        public VisualizerForm()
        {
            var startGameConfigPanel = new StartGameConfigPanel
            {
                Dock = DockStyle.Left
            };
            startGameConfigPanel.SetMaps(MapLoader.LoadDefaultMaps().ToArray());

            var painter = new MapPainter
            {
                Map = startGameConfigPanel.SelectedMap.Map,
                PainterAugmentor = new DefaultPainterAugmentor
                {
                    Map = startGameConfigPanel.SelectedMap.Map
                }
            };

            var panel = new ScaledViewPanel(painter)
            {
                Dock = DockStyle.Fill
            };
            startGameConfigPanel.MapChanged += map =>
            {
                painter.Map = map.Map;
                panel.Refresh();
            };


            Controls.Add(startGameConfigPanel);
            Controls.Add(panel);
        }
    }

    [TestFixture]
    public class VisualizerForm_Test
    {
        [Explicit]
        [Test]
        [STAThread]
        public void Test()
        {
            var form = new VisualizerForm();
            form.ShowDialog();
        }
    }
}