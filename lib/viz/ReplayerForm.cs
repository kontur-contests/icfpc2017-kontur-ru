﻿using System.Collections.Generic;
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
                Dock = DockStyle.Fill,
                Repo = repo
            };

            var rightPanel = new ReplayerPanel()
            {
                Dock = DockStyle.Fill,
                ShowScore = false
            };
            //UpdateMap(rightPanel);
            selectReplayPanel.ReplayChanged += () =>
            {
                UpdateMap(rightPanel);
            };

            var split = new SplitContainer()
            {
                Dock=DockStyle.Fill,
                Orientation = Orientation.Vertical
            };
            split.Panel1.Controls.Add(selectReplayPanel);
            split.Panel2.Controls.Add(rightPanel);
            Controls.Add(split);
        }

        private void UpdateMap(ReplayerPanel rightPanel)
        {
            rightPanel.SetDataProvider(selectReplayPanel.SelectedReplay.Data.Map, new LogReplayDataProvider(selectReplayPanel.SelectedReplay));
        }
    }
}