using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using lib.Structures;
using NUnit.Framework;

namespace lib.viz
{
    public class VisualizerForm : Form
    {
        private readonly StartGameConfigPanel startGameConfigPanel;

        public VisualizerForm()
        {
            startGameConfigPanel = new StartGameConfigPanel
            {
                Dock = DockStyle.Left
            };
            startGameConfigPanel.SetMaps(MapLoader.LoadDefaultMaps().ToArray());
            startGameConfigPanel.SetAis(AiFactoryRegistry.Factories);


            var rightPanel = new ReplayerPanel
            {
                Dock = DockStyle.Fill
            };
            var map = startGameConfigPanel.SelectedMap.Map;
            UpdateMap(rightPanel);
            startGameConfigPanel.MapChanged += changedMap =>
            {
                UpdateMap(rightPanel);
            };

            startGameConfigPanel.AiSelected += factory =>
            {
                UpdateMap(rightPanel);
            };
            startGameConfigPanel.AiAtIndexRemoved += _ => UpdateMap(rightPanel);

            startGameConfigPanel.EnableFuturesChanged += _ => UpdateMap(rightPanel);
            startGameConfigPanel.EnableSplurgesChanged += _ => UpdateMap(rightPanel);
            startGameConfigPanel.EnableOptionsChanged += _ => UpdateMap(rightPanel);

            Controls.Add(rightPanel);
            Controls.Add(startGameConfigPanel);
        }

        private void UpdateMap(ReplayerPanel rightPanel)
        {
            var map = startGameConfigPanel.SelectedMap.Map;
            var settings = startGameConfigPanel.Settings;
            rightPanel.SetDataProvider(map, new SimulatorReplayDataProvider(startGameConfigPanel.SelectedAis, map, settings));
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