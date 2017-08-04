using System;
using System.Linq;
using System.Windows.Forms;
using lib.Strategies;
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
            startGameConfigPanel.SetAis(AiFactoryRegistry.Factories);

            var painter = new MapPainter
            {
                Map = startGameConfigPanel.SelectedMap.Map,
                PainterAugmentor = new DefaultPainterAugmentor()
            };

            var panel = new ScaledViewPanel(painter)
            {
                Dock = DockStyle.Fill
            };
            GameSimulator simulator = new GameSimulator(startGameConfigPanel.SelectedMap.Map);

            startGameConfigPanel.MapChanged += map =>
            {
                simulator = new GameSimulator(startGameConfigPanel.SelectedMap.Map);
                painter.Map = map.Map;
                panel.Refresh();
            };

            var makeStepButton = new Button
            {
                Dock = DockStyle.Bottom,
                Text = "MAKE STEP"
            };

            startGameConfigPanel.AiSelected += factory =>
            {
                simulator.StartGame(startGameConfigPanel.SelectedAis);
            };

            makeStepButton.Click += (sender, args) =>
            {
                painter.Map = simulator.NextMove().CurrentMap;
                panel.Refresh();
            };

            Controls.Add(startGameConfigPanel);
            Controls.Add(panel);
            Controls.Add(makeStepButton);
        }
    }

    public class AiFactoryRegistry
    {
        public static AiFactory[] Factories => new[]
        {
            new AiFactory("GreedyAi", () => new GreedyAi()),
        };
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