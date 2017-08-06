using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using lib.Replays;
using MoreLinq;

namespace lib.viz
{
    internal class SelectReplayPanel : Panel
    {
        public SelectReplayPanel()
        {
            filter = new FilterReplayPanel();
            filter.FiltersUpdated += UpdateVisualization;
            var buttonsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Top
            };
            var refreshButton = new Button()
            {
                Text = "Refresh",
                AutoSize = true
            };
            refreshButton.Click += (sender, args) => RefreshMetasList();
            buttonsPanel.Controls.Add(filter);
            buttonsPanel.Controls.Add(refreshButton);
            debugTextArea = new TextBox()
            {
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 12),
                Dock = DockStyle.Bottom,
                Text = "",
                Height = 400,
                ScrollBars = ScrollBars.Vertical
            };
            listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false,
                Width = 600
            };

            listView.FullRowSelect = true;
            listView.HideSelection = false;

            listView.Columns.Add("Time");
            listView.Columns.Add("Ai");
            listView.Columns.Add("Points per game (max = PuntersCount)");
            listView.Columns.Add("PuntersCount");


            listView.ItemSelectionChanged += SelectedReplayChanged;
            Controls.Add(listView);
            Controls.Add(debugTextArea);
            Controls.Add(buttonsPanel);
        }

        private void SelectedReplayChanged(object sender, ListViewItemSelectionChangedEventArgs args)
        {
            if (repo == null || listView.SelectedItems.Count == 0) return;
            var lvItem = listView.SelectedItems[0];
            var meta = (ReplayMeta)lvItem.Tag;
            var data = repo.GetData(meta);
            debugTextArea.Text = meta.ToString() + "\r\n\r\n" + data.Moves.ToDelimitedString("\r\n");
            SelectedReplay = new ReplayFullData(meta, data);
            ReplayChanged?.Invoke();
        }

        private ReplayRepo repo;
        private ListView listView;
        private TextBox debugTextArea;
        private FilterReplayPanel filter;

        public ReplayRepo Repo
        {
            get { return repo; }
            set
            {
                repo = value;
                RefreshMetasList();
            }
        }

        private void RefreshMetasList()
        {
            var metas = repo.GetRecentMetas(50);
            UpdateList(metas);
        }

        private void UpdateList(ReplayMeta[] metas)
        {
            filter.UpdateMetas(metas);
        }

        private void UpdateVisualization(ReplayMeta[] metas)
        {
            listView.BeginUpdate();
            listView.Items.Clear();
            foreach (var meta in metas)
            {
                var lvItem = listView.Items.Add(meta.Timestamp.ToString("T"));
                lvItem.Tag = meta;
                lvItem.SubItems.Add($"{meta.AiName}:{meta.AiVersion}");
                var ourScore = meta.Scores.First(s => s.punter == meta.OurPunter).score;
                var count = meta.Scores.Count(s => s.score < ourScore) + 1;
                lvItem.SubItems.Add(count.ToString());
                lvItem.SubItems.Add(meta.Scores.Length.ToString());
                lvItem.BackColor = (count == meta.Scores.Length) ? Color.GreenYellow : Color.White;
            }
            listView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView.EndUpdate();
        }

        public ReplayFullData SelectedReplay { get; private set; }

        public event Action ReplayChanged;
    }
}