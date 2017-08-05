using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using lib.Replays;

namespace lib.viz
{
    public class ReplayerForm : Form
    {
        private readonly SelectReplayPanel selectReplayPanel;

        public ReplayerForm(ReplayRepo repo = null)
        {
            Size = new Size(800, 600);
            selectReplayPanel = new SelectReplayPanel
            {
                Dock = DockStyle.Left,
                Repo = repo
            };

            var rightPanel = new ReplayerPanel()
            {
                Dock = DockStyle.Fill
            };
            //UpdateMap(rightPanel);
            selectReplayPanel.ReplayChanged += () =>
            {
                UpdateMap(rightPanel);
            };

            Controls.Add(rightPanel);
            Controls.Add(selectReplayPanel);
        }

        private void UpdateMap(ReplayerPanel rightPanel)
        {
            rightPanel.SetDataProvider(selectReplayPanel.SelectedReplay.Data.Map, new LogReplayDataProvider(selectReplayPanel.SelectedReplay));
        }
    }
}